using CRM.Application.Common.Interfaces;
using CRM.Application.Common.Models;
using CRM.Application.StockMovements.Dtos;
using CRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CRM.Application.StockMovements;

public class StockMovementService : IStockMovementService
{
    private readonly IApplicationDbContext _context;

    public StockMovementService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<StockMovementResult?> ImportAsync(int productId, ImportStockRequest request)
    {
        var product = await _context.Products.FindAsync(productId);
        if (product is null)
        {
            return null;
        }

        await using var transaction = await _context.BeginTransactionAsync();
        try
        {
            product.StockQuantity += request.Quantity;

            var movement = new StockMovement
            {
                ProductId = productId,
                Type = StockMovementType.Import,
                Quantity = request.Quantity,
                Reference = null, // nhập thủ công, không gắn Order nào
                CreatedAtUtc = DateTime.UtcNow,
                Note = request.Note,
            };
            _context.StockMovements.Add(movement);

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return new StockMovementResult(true, movement.ToDto(product), null);
        }
        catch (DbUpdateConcurrencyException)
        {
            return new StockMovementResult(
                false,
                null,
                "Sản phẩm vừa được cập nhật bởi giao dịch khác, vui lòng thử lại.",
                IsConflict: true
            );
        }
    }

    public async Task<StockMovementResult?> AdjustAsync(int productId, AdjustStockRequest request)
    {
        var product = await _context.Products.FindAsync(productId);
        if (product is null)
        {
            return null;
        }

        // Áp lại nguyên tắc "không âm kho" (Ngày 17) — invariant này đúng cho mọi cách làm thay
        // đổi StockQuantity, không riêng gì CreateOrder.
        if (product.StockQuantity + request.Quantity < 0)
        {
            return new StockMovementResult(
                false,
                null,
                $"Điều chỉnh sẽ làm kho âm (hiện {product.StockQuantity}, điều chỉnh {request.Quantity})."
            );
        }

        await using var transaction = await _context.BeginTransactionAsync();
        try
        {
            product.StockQuantity += request.Quantity;

            var movement = new StockMovement
            {
                ProductId = productId,
                Type = StockMovementType.Adjustment,
                Quantity = request.Quantity,
                Reference = null,
                CreatedAtUtc = DateTime.UtcNow,
                Note = request.Note,
            };
            _context.StockMovements.Add(movement);

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return new StockMovementResult(true, movement.ToDto(product), null);
        }
        catch (DbUpdateConcurrencyException)
        {
            return new StockMovementResult(
                false,
                null,
                "Sản phẩm vừa được cập nhật bởi giao dịch khác, vui lòng thử lại.",
                IsConflict: true
            );
        }
    }

    public async Task<PageResult<StockMovementDto>?> GetHistoryAsync(int productId, StockMovementQueryParams queryParams)
    {
        var product = await _context.Products.FindAsync(productId);
        if (product is null)
        {
            return null;
        }

        var query = _context.StockMovements.Where(m => m.ProductId == productId);

        if (queryParams.Type is StockMovementType type)
        {
            query = query.Where(m => m.Type == type);
        }

        if (queryParams.From is DateTime from)
        {
            query = query.Where(m => m.CreatedAtUtc >= from);
        }

        if (queryParams.To is DateTime to)
        {
            query = query.Where(m => m.CreatedAtUtc <= to);
        }

        query = query.OrderByDescending(m => m.CreatedAtUtc);

        var totalCount = await query.CountAsync();

        // Lấy entity thật ra trước (ToListAsync), rồi mới .Select(...ToDto()) trên danh sách
        // trong bộ nhớ — vì ToDto() gọi Type.ToString() trên enum, EF Core không dịch được thẳng
        // thành SQL nếu gộp chung vào Select() của IQueryable.
        var movements = await query
            .Skip((queryParams.Page - 1) * queryParams.PageSize)
            .Take(queryParams.PageSize)
            .ToListAsync();

        var items = movements.Select(m => m.ToDto(product)).ToList();

        return new PageResult<StockMovementDto>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = queryParams.Page,
            PageSize = queryParams.PageSize,
        };
    }
}
