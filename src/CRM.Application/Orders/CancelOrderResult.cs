using CRM.Application.Orders.Dtos;
namespace CRM.Application.Orders;

public record CancelOrderResult(bool Succeeded, OrderDto? Order, string? Error);
