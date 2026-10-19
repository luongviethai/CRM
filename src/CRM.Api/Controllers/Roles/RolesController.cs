using CRM.Application.Administration;
using CRM.Application.Administration.Dtos;
using CRM.Infrastructure.Identity;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRM.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RolesController : ControllerBase
{
    private readonly IRoleManagementService _roleManagementService;
    private readonly IValidator<CreateRoleRequest> _createRoleValidator;
    private readonly IValidator<AssignPermissionRequest> _assignPermissionValidator;

    public RolesController(
        IRoleManagementService roleManagementService,
        IValidator<CreateRoleRequest> createRoleValidator,
        IValidator<AssignPermissionRequest> assignPermissionValidator)
    {
        _roleManagementService = roleManagementService;
        _createRoleValidator = createRoleValidator;
        _assignPermissionValidator = assignPermissionValidator;
    }

    [Authorize(Policy = PermissionCatalog.RoleManage)]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var roles = await _roleManagementService.GetAllRolesAsync();
        return Ok(roles);
    }

    [Authorize(Policy = PermissionCatalog.RoleManage)]
    [HttpGet("{id:int}", Name = "GetRoleById")]
    public async Task<IActionResult> GetById(int id)
    {
        var role = await _roleManagementService.GetRoleByIdAsync(id);

        if (role is null)
        {
            return NotFound();
        }

        return Ok(role);
    }

    [Authorize(Policy = PermissionCatalog.RoleManage)]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateRoleRequest request)
    {
        var validationResult = await _createRoleValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        var result = await _roleManagementService.CreateRoleAsync(request.Name);
        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }

        return CreatedAtRoute("GetRoleById", new { id = result.Role!.Id }, result.Role);
    }

    // permission.manage (không phải role.manage) — đúng theo bảng endpoint ở tài liệu nghiệp vụ
    // mục 9: gán permission cho role là hành động riêng, tách khỏi quản lý role.
    [Authorize(Policy = PermissionCatalog.PermissionManage)]
    [HttpPost("{id:int}/permissions")]
    public async Task<IActionResult> AssignPermission(int id, [FromBody] AssignPermissionRequest request)
    {
        var validationResult = await _assignPermissionValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        var succeeded = await _roleManagementService.AssignPermissionAsync(id, request.PermissionName);
        if (!succeeded)
        {
            return NotFound();
        }

        return NoContent();
    }

    [Authorize(Policy = PermissionCatalog.PermissionManage)]
    [HttpDelete("{id:int}/permissions/{permissionName}")]
    public async Task<IActionResult> RemovePermission(int id, string permissionName)
    {
        var succeeded = await _roleManagementService.RemovePermissionAsync(id, permissionName);
        if (!succeeded)
        {
            return NotFound();
        }

        return NoContent();
    }
}
