using CRM.Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace CRM.Infrastructure.Authorization;

public class PermissionPolicyProvider : IAuthorizationPolicyProvider
{
    private readonly DefaultAuthorizationPolicyProvider _fallbackProvider;

    public PermissionPolicyProvider(IOptions<AuthorizationOptions> options)
    {
        _fallbackProvider = new DefaultAuthorizationPolicyProvider(options);
    }

    // [Authorize] trơn, không chỉ định Policy -> giữ nguyên hành vi mặc định (yêu cầu đã đăng nhập).
    public Task<AuthorizationPolicy> GetDefaultPolicyAsync() => _fallbackProvider.GetDefaultPolicyAsync();

    // Không có [Authorize] nào cả nhưng có cấu hình FallbackPolicy toàn cục -> project này không
    // dùng, giữ nguyên hành vi mặc định (null).
    public Task<AuthorizationPolicy?> GetFallbackPolicyAsync() => _fallbackProvider.GetFallbackPolicyAsync();

    public Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        if (!PermissionCatalog.All.Contains(policyName))
        {
            // Không phải tên permission hợp lệ -> không phải việc của mình, để default provider
            // xử lý (thường trả null -> ASP.NET Core tự báo lỗi "policy not found" ngay).
            return _fallbackProvider.GetPolicyAsync(policyName);
        }

        var policy = new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .AddRequirements(new PermissionRequirement(policyName))
            .Build();

        return Task.FromResult<AuthorizationPolicy?>(policy);
    }
}
