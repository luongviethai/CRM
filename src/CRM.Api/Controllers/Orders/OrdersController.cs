using System.Security.Claims;
using CRM.Application.Orders;
using CRM.Application.Orders.Dtos;
using CRM.Infrastructure.Identity;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRM.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;
    private readonly IValidator<CreateOrderRequest> _createOrderValidator;

    public OrdersController(IOrderService orderService, IValidator<CreateOrderRequest> createOrderValidator)
    {
        _orderService = orderService;
        _createOrderValidator = createOrderValidator;
    }

    [Authorize(Policy = PermissionCatalog.OrderCreate)]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateOrderRequest request)
    {
        var validationResult = await _createOrderValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        var createdByUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var result = await _orderService.CreateOrderAsync(request, createdByUserId);
        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }

        return StatusCode(StatusCodes.Status201Created, result.Order);
    }

    [Authorize(Policy = PermissionCatalog.OrderCancel)]
    [HttpPost("{id:int}/cancel")]

    public async Task<IActionResult> Cancel(int id)
    {
        var result = await _orderService.CancelOrderAsync(id);

        if (result is null)
        {
            return NotFound();
        }

        if (!result.Succeeded)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Order);
    }
}

