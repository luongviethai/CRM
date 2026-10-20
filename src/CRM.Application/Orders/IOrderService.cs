// IOrderService.cs
using CRM.Application.Orders.Dtos;
namespace CRM.Application.Orders;

public interface IOrderService
{
    Task<CreateOrderResult> CreateOrderAsync(CreateOrderRequest request, int createdByUserId);
}
