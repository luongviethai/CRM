// CreateOrderResult.cs
using CRM.Application.Orders.Dtos;
namespace CRM.Application.Orders;

public record CreateOrderResult(bool Succeeded, OrderDto? Order, IReadOnlyList<string> Errors);
