using CRM.Application.Administration;
using CRM.Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRM.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PermissionsController : ControllerBase
{
    private readonly IRoleManagementService _roleManagementService;

    public PermissionsController(IRoleManagementService roleManagementService)
    {
        _roleManagementService = roleManagementService;
    }

    // Danh sách tên permission hợp lệ — admin dùng để biết có gì mà gán cho role
    // (POST /api/roles/{id}/permissions), không cần tra code PermissionCatalog.
    [Authorize(Policy = PermissionCatalog.PermissionManage)]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var permissions = await _roleManagementService.GetAllPermissionNamesAsync();
        return Ok(permissions);
    }
}
