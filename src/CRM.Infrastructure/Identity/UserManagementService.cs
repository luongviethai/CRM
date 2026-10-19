using CRM.Application.Administration;
using CRM.Application.Administration.Dtos;
using CRM.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CRM.Infrastructure.Identity;

public class UserManagementService : IUserManagementService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly AppDbContext _context;

    public UserManagementService(UserManager<ApplicationUser> userManager, AppDbContext context)
    {
        _userManager = userManager;
        _context = context;
    }

    public async Task<IReadOnlyList<UserSummaryDto>> GetAllUsersAsync()
    {
        var users = await _context.Users.ToListAsync();

        // Join trực tiếp UserRoles/Roles thay vì gọi UserManager.GetRolesAsync() cho từng user
        // trong vòng lặp — tránh N+1 (bài học từ Ngày 7).
        var userRoles = await (
            from ur in _context.UserRoles
            join r in _context.Roles on ur.RoleId equals r.Id
            select new { ur.UserId, RoleName = r.Name! }
        ).ToListAsync();

        return users
            .Select(u => new UserSummaryDto(
                u.Id,
                u.Email!,
                u.FullName,
                userRoles.Where(ur => ur.UserId == u.Id).Select(ur => ur.RoleName).ToList()))
            .ToList();
    }

    public async Task<bool> AssignRoleAsync(int userId, string roleName)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user is null)
        {
            return false;
        }

        // UserManager.AddToRoleAsync KHÔNG trả IdentityResult.Failed khi role không tồn tại
        // như các hàm Identity khác — nó THROW InvalidOperationException. Phải tự check tồn
        // tại trước, nếu không 1 role name gõ sai sẽ làm API trả 500 thay vì 404.
        var roleExists = await _context.Roles.AnyAsync(r => r.Name == roleName);
        if (!roleExists)
        {
            return false;
        }

        if (await _userManager.IsInRoleAsync(user, roleName))
        {
            return true; // idempotent
        }

        var result = await _userManager.AddToRoleAsync(user, roleName);
        return result.Succeeded;
    }

    public async Task<bool> RemoveRoleAsync(int userId, string roleName)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user is null)
        {
            return false;
        }

        if (!await _userManager.IsInRoleAsync(user, roleName))
        {
            return false; // user này đâu có role đó, không có gì để xóa
        }

        var result = await _userManager.RemoveFromRoleAsync(user, roleName);
        return result.Succeeded;
    }
}
