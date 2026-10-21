using CRM.Application.Common.Interfaces;
using CRM.Application.Orders.Dtos;
using CRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CRM.Application.Orders;

public class OrderService : IOrderService
{
    private readonly IApplicationDbContext _context;

    public OrderService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<CreateOrderResult> CreateOrderAsync(
        CreateOrderRequest request,
        int createdByUserId
    )
    {
        // --- Bước 1: load dữ liệu thật + check tồn tại — làm NGOÀI transaction vì chỉ đọc,
        // không cần giữ lock DB cho phần này. ---
        var customer = await _context.Customers.FindAsync(request.CustomerId);

        var productIds = request.Items.Select(i => i.ProductId).Distinct().ToList();

        var products = await _context
            .Products.Where(p => productIds.Contains(p.Id))
            .ToDictionaryAsync(p => p.Id);

        var errors = new List<string>();

        if (customer is null)
        {
            errors.Add($"Customer #{request.CustomerId} không tồn tại.");
        }
        foreach (var item in request.Items)
        {
            if (!products.TryGetValue(item.ProductId, out var product))
            {
                errors.Add($"Product #{item.ProductId} không tồn tại.");
                continue;
            }

            if (product.StockQuantity < item.Quantity)
            {
                errors.Add(
                    $"Product #{item.ProductId} không đủ hàng (còn {product.StockQuantity}, cần {item.Quantity})."
                );
            }
        }

        if (errors.Count > 0)
        {
            return new CreateOrderResult(false, null, errors);
        }

        // --- Bước 2: mở transaction, bắt đầu phần GHI ---
        await using var transaction = await _context.BeginTransactionAsync();

        try
        {
            var order = new Order
            {
                OrderCode = GenerateOrderCode(),
                CustomerId = request.CustomerId,
                OrderDate = DateTime.UtcNow,
                Status = OrderStatus.Pending,
                Notes = request.Notes,
                CreatedBy = createdByUserId,
            };

            foreach (var item in request.Items)
            {
                var product = products[item.ProductId];

                order.OrderItems.Add(
                    new OrderItem
                    {
                        ProductId = product.Id,
                        Quantity = item.Quantity,
                        UnitPrice = product.Price, // chốt giá tại thời điểm đặt hàng, không tham chiếu Product.Price sau này
                    }
                );

                product.StockQuantity -= item.Quantity;
            }

            order.TotalAmount = order.OrderItems.Sum(oi => oi.TotalPrice);

            _context.Orders.Add(order);
            await _context.SaveChangesAsync(); // (1) Order.Id có giá trị thật sau dòng này — đây cũng là lần UPDATE Product, chỗ concurrency token (xmin) được kiểm tra

            foreach (var item in order.OrderItems)
            {
                _context.StockMovements.Add(
                    new StockMovement
                    {
                        ProductId = item.ProductId,
                        Type = StockMovementType.Export,
                        Quantity = -item.Quantity, // âm vì kho giảm
                        Reference = order.Id, // giờ mới có Order.Id thật để gán
                        CreatedAtUtc = DateTime.UtcNow,
                    }
                );
            }

            await _context.SaveChangesAsync(); // (2)

            await transaction.CommitAsync();

            return new CreateOrderResult(true, order.ToDto(customer!), []);
        }
        catch (DbUpdateConcurrencyException)
        {
            // await using transaction tự rollback khi dispose (chưa CommitAsync) — không cần tự
            // gọi RollbackAsync() ở đây.
            return new CreateOrderResult(
                false,
                null,
                ["Một sản phẩm trong đơn vừa được cập nhật bởi giao dịch khác, vui lòng thử lại."],
                IsConflict: true
            );
        }
    }

    public async Task<CancelOrderResult?> CancelOrderAsync(int orderId)
    {
        var order = await _context
            .Orders.Include(o => o.Customer)
            .Include(o => o.OrderItems)
            .FirstOrDefaultAsync(o => o.Id == orderId);

        if (order is null)
        {
            return null;
        }

        if (order.Status != OrderStatus.Pending)
        {
            return new CancelOrderResult(false, null, $"Không thể hủy đơn hàng ở trạng thái {order.Status}");
        }

        var productIds = order.OrderItems.Select(oi => oi.ProductId).ToList();

        var products = await _context.Products
            .Where(p => productIds.Contains(p.Id))
            .ToDictionaryAsync(p => p.Id);

        await using var transaction = await _context.BeginTransactionAsync();

        foreach (var item in order.OrderItems)
        {
            var product = products[item.ProductId];
            product.StockQuantity += item.Quantity;

            _context.StockMovements.Add(new StockMovement
            {
                ProductId = item.ProductId,
                Type = StockMovementType.Adjustment,
                Quantity = item.Quantity, // dương vì kho tăng (hoàn kho)
                Reference = order.Id,     // Order đã tồn tại từ đầu -> có Id ngay, không cần SaveChanges trước
                CreatedAtUtc = DateTime.UtcNow,
                Note = "Hoàn kho do hủy đơn"
            });
        }

        order.Status = OrderStatus.Cancelled;

        await _context.SaveChangesAsync(); // 1 lần duy nhất là đủ — khác Ngày 15

        await transaction.CommitAsync();

        return new CancelOrderResult(true, order.ToDto(order.Customer), null);
    }

    private static string GenerateOrderCode() =>
        $"ORD-{DateTime.UtcNow:yyyyMMddHHmmss}-{Random.Shared.Next(1000, 9999)}";
}
