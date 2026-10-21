using CRM.Application.StockMovements;
using CRM.Application.StockMovements.Dtos;
using CRM.Infrastructure.Identity;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRM.Api.Controllers;

[ApiController]
[Route("api/products/{productId:int}/stock-movements")]
public class StockMovementsController : ControllerBase
{
    private readonly IStockMovementService _stockMovementService;
    private readonly IValidator<ImportStockRequest> _importValidator;
    private readonly IValidator<AdjustStockRequest> _adjustValidator;

    public StockMovementsController(
        IStockMovementService stockMovementService,
        IValidator<ImportStockRequest> importValidator,
        IValidator<AdjustStockRequest> adjustValidator)
    {
        _stockMovementService = stockMovementService;
        _importValidator = importValidator;
        _adjustValidator = adjustValidator;
    }

    [Authorize(Policy = PermissionCatalog.StockMovementManual)]
    [HttpPost("import")]
    public async Task<IActionResult> Import(int productId, [FromBody] ImportStockRequest request)
    {
        var validationResult = await _importValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        var result = await _stockMovementService.ImportAsync(productId, request);
        if (result is null)
        {
            return NotFound();
        }

        if (!result.Succeeded)
        {
            return result.IsConflict ? Conflict(result.Error) : BadRequest(result.Error);
        }

        return StatusCode(StatusCodes.Status201Created, result.Movement);
    }

    [Authorize(Policy = PermissionCatalog.StockMovementManual)]
    [HttpPost("adjustment")]
    public async Task<IActionResult> Adjust(int productId, [FromBody] AdjustStockRequest request)
    {
        var validationResult = await _adjustValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        var result = await _stockMovementService.AdjustAsync(productId, request);
        if (result is null)
        {
            return NotFound();
        }

        if (!result.Succeeded)
        {
            return result.IsConflict ? Conflict(result.Error) : BadRequest(result.Error);
        }

        return StatusCode(StatusCodes.Status201Created, result.Movement);
    }

    [Authorize(Policy = PermissionCatalog.StockMovementView)]
    [HttpGet]
    public async Task<IActionResult> GetHistory(int productId, [FromQuery] StockMovementQueryParams queryParams)
    {
        var result = await _stockMovementService.GetHistoryAsync(productId, queryParams);
        if (result is null)
        {
            return NotFound();
        }

        return Ok(result);
    }
}
