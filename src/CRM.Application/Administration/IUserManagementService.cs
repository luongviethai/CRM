using CRM.Application.Administration.Dtos;

namespace CRM.Application.Administration;

public interface IUserManagementService
{
    Task<IReadOnlyList<UserSummaryDto>> GetAllUsersAsync();
    Task<bool> AssignRoleAsync(int userId, string roleName);
    Task<bool> RemoveRoleAsync(int userId, string roleName);
}
