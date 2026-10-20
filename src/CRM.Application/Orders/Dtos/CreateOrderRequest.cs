namespace CRM.Application.Orders.Dtos;

public record CreateOrderRequest(int CustomerId, List<CreateOrderItemRequest> Items, string? Notes);

public record CreateOrderItemRequest(int ProductId, int Quantity);