using CRM.Application.Administration.Dtos;

namespace CRM.Application.Administration;

public interface IRoleManagementService
{
    Task<IReadOnlyList<RoleDto>> GetAllRolesAsync();
    Task<RoleDto?> GetRoleByIdAsync(int id);
    Task<CreateRoleResult> CreateRoleAsync(string name);
    Task<bool> AssignPermissionAsync(int roleId, string permissionName);
    Task<bool> RemovePermissionAsync(int roleId, string permissionName);
    Task<IReadOnlyList<string>> GetAllPermissionNamesAsync();
}
