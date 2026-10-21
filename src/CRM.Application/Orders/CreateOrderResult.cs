// CreateOrderResult.cs
using CRM.Application.Orders.Dtos;
namespace CRM.Application.Orders;

// IsConflict = true khi thất bại do DbUpdateConcurrencyException (có request khác sửa trước) ->
// controller trả 409, khác với các lỗi nghiệp vụ thường (Customer/Product không tồn tại, thiếu
// hàng) trả 400.
public record CreateOrderResult(
    bool Succeeded,
    OrderDto? Order,
    IReadOnlyList<string> Errors,
    bool IsConflict = false
);
