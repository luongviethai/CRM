using CRM.Application.Customers;
using CRM.Application.Customers.Dtos;
using CRM.Infrastructure.Identity;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRM.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly ICustomerService _customerService;
    private readonly IValidator<CreateCustomerRequest> _createValidator;
    private readonly IValidator<UpdateCustomerRequest> _updateValidator;

    public CustomersController(
        ICustomerService customerService,
        IValidator<CreateCustomerRequest> createValidator,
        IValidator<UpdateCustomerRequest> updateValidator)
    {
        _customerService = customerService;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    [Authorize(Policy = PermissionCatalog.CustomerView)]
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] CustomerQueryParams? queryParams)
    {
        var customers = await _customerService.GetAllAsync(queryParams);
        return Ok(customers);
    }

    [Authorize(Policy = PermissionCatalog.CustomerView)]
    [HttpGet("{id:int}", Name = "GetCustomerById")]
    public async Task<IActionResult> GetByIdAsync(int id)
    {
        var customer = await _customerService.GetByIdAsync(id);

        if (customer == null)
        {
            return NotFound();
        }
        return Ok(customer);
    }

    [Authorize(Policy = PermissionCatalog.CustomerManage)]
    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] CreateCustomerRequest request)
    {
        var validationResult = await _createValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        var customer = await _customerService.CreateAsync(request);

        return CreatedAtRoute("GetCustomerById", new { id = customer.Id }, customer);
    }

    [Authorize(Policy = PermissionCatalog.CustomerManage)]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateAsync(int id, [FromBody] UpdateCustomerRequest request)
    {
        var validationResult = await _updateValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        var updatedCustomer = await _customerService.UpdateAsync(id, request);

        if (updatedCustomer == null)
        {
            return NotFound();
        }

        return Ok(updatedCustomer);
    }

    // Ma trận (mục 9 tài liệu nghiệp vụ) chỉ liệt kê GET/POST/PUT cho /api/customers, nhưng
    // DeleteAsync vẫn tồn tại — áp dụng chung customer.manage như cách CategoriesController đã
    // làm với category.manage, vì không có permission "customer.delete" riêng.
    [Authorize(Policy = PermissionCatalog.CustomerManage)]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteAsync(int id)
    {
        var result = await _customerService.DeleteAsync(id);

        if (!result)
        {
            return NotFound();
        }

        return NoContent();
    }
}
