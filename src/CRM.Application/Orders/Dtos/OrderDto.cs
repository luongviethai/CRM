namespace CRM.Application.Orders.Dtos;

public record OrderDto(
    int Id,
    string OrderCode,
    int CustomerId,
    string CustomerName,
    DateTime OrderDate,
    decimal TotalAmount,
    string Status,
    string? Note,
    IReadOnlyList<OrderItemDto> Items
);

public record OrderItemDto(
    int ProductId,
    string ProductName,
    int Quantity,
    decimal UnitPrice,
    decimal TotalPrice
);
