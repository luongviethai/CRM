using Microsoft.AspNetCore.Authorization;

namespace CRM.Infrastructure.Authorization;

// Chỉ là data — nói "cần permission này", không tự biết cách kiểm tra. Logic thật nằm ở
// PermissionAuthorizationHandler.
public class PermissionRequirement : IAuthorizationRequirement
{
    public string Permission { get; }

    public PermissionRequirement(string permission)
    {
        Permission = permission;
    }
}
