// OrderMappingExtensions.cs
using CRM.Application.Orders.Dtos;
using CRM.Domain.Entities;

namespace CRM.Application.Orders;

public static class OrderMappingExtensions
{
    public static OrderDto ToDto(this Order order, Customer customer)
    {
        return new OrderDto(
            order.Id,
            order.OrderCode,
            order.CustomerId,
            customer.Name,
            order.OrderDate,
            order.TotalAmount,
            order.Status.ToString(),
            order.Notes,
            order.OrderItems.Select(oi => new OrderItemDto(
                oi.ProductId,
                oi.Product?.Name ?? string.Empty,
                oi.Quantity,
                oi.UnitPrice,
                oi.TotalPrice)).ToList()
        );
    }
}
