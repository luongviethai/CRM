using CRM.Application.Administration;
using CRM.Application.Administration.Dtos;
using CRM.Infrastructure.Identity;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRM.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserManagementService _userManagementService;
    private readonly IValidator<AssignRoleRequest> _assignRoleValidator;

    public UsersController(
        IUserManagementService userManagementService,
        IValidator<AssignRoleRequest> assignRoleValidator)
    {
        _userManagementService = userManagementService;
        _assignRoleValidator = assignRoleValidator;
    }

    [Authorize(Policy = PermissionCatalog.UserManage)]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var users = await _userManagementService.GetAllUsersAsync();
        return Ok(users);
    }

    [Authorize(Policy = PermissionCatalog.UserManage)]
    [HttpPost("{id:int}/roles")]
    public async Task<IActionResult> AssignRole(int id, [FromBody] AssignRoleRequest request)
    {
        var validationResult = await _assignRoleValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        var succeeded = await _userManagementService.AssignRoleAsync(id, request.RoleName);
        if (!succeeded)
        {
            return NotFound();
        }

        return NoContent();
    }

    [Authorize(Policy = PermissionCatalog.UserManage)]
    [HttpDelete("{id:int}/roles/{roleName}")]
    public async Task<IActionResult> RemoveRole(int id, string roleName)
    {
        var succeeded = await _userManagementService.RemoveRoleAsync(id, roleName);
        if (!succeeded)
        {
            return NotFound();
        }

        return NoContent();
    }
}
