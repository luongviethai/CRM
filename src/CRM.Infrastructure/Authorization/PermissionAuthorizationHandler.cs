using System.Security.Claims;
using CRM.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace CRM.Infrastructure.Authorization;

public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    private readonly AppDbContext _context;

    public PermissionAuthorizationHandler(AppDbContext context)
    {
        _context = context;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        var roleNames = context.User
            .FindAll(ClaimTypes.Role)
            .Select(c => c.Value)
            .ToList();

        if (roleNames.Count == 0)
        {
            return; // không có role nào -> chắc chắn không có permission nào, để requirement treo (= deny)
        }

        var hasPermission = await _context.RolePermissions
            .AnyAsync(rp => roleNames.Contains(rp.Role.Name!) && rp.Permission.Name == requirement.Permission);

        if (hasPermission)
        {
            context.Succeed(requirement);
        }

        // Không gọi gì nếu sai: KHÔNG tự ý context.Fail() — để các handler khác (nếu sau này
        // có thêm, ví dụ permission qua claim trực tiếp) vẫn có cơ hội tự Succeed. Không ai
        // Succeed -> requirement coi như chưa thỏa -> 403 mặc định.
    }
}
