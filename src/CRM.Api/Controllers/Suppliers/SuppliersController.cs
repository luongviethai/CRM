using CRM.Application.Suppliers;
using CRM.Application.Suppliers.Dtos;
using CRM.Infrastructure.Identity;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRM.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SuppliersController : ControllerBase
{
    private readonly ISupplierService _supplierService;
    private readonly IValidator<CreateSupplierRequest> _createValidator;
    private readonly IValidator<UpdateSupplierRequest> _updateValidator;

    public SuppliersController(
        ISupplierService supplierService,
        IValidator<CreateSupplierRequest> createValidator,
        IValidator<UpdateSupplierRequest> updateValidator)
    {
        _supplierService = supplierService;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    [Authorize(Policy = PermissionCatalog.SupplierView)]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var suppliers = await _supplierService.GetAllAsync();
        return Ok(suppliers);
    }

    [Authorize(Policy = PermissionCatalog.SupplierView)]
    [HttpGet("{id:int}", Name = "GetSupplierById")]
    public async Task<IActionResult> GetByIdAsync(int id)
    {
        var supplier = await _supplierService.GetByIdAsync(id);

        if (supplier == null)
        {
            return NotFound();
        }
        return Ok(supplier);
    }

    [Authorize(Policy = PermissionCatalog.SupplierManage)]
    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] CreateSupplierRequest request)
    {
        var validationResult = await _createValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        var supplier = await _supplierService.CreateAsync(request);

        return CreatedAtRoute("GetSupplierById", new { id = supplier.Id }, supplier);
    }

    [Authorize(Policy = PermissionCatalog.SupplierManage)]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateAsync(int id, [FromBody] UpdateSupplierRequest request)
    {
        var validationResult = await _updateValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        var updatedSupplier = await _supplierService.UpdateAsync(id, request);

        if (updatedSupplier == null)
        {
            return NotFound();
        }

        return Ok(updatedSupplier);
    }

    [Authorize(Policy = PermissionCatalog.SupplierManage)]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteAsync(int id)
    {
        var result = await _supplierService.DeleteAsync(id);

        if (!result)
        {
            return NotFound();
        }

        return NoContent();
    }
}
