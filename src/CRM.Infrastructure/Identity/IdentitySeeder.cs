using Microsoft.AspNetCore.Identity;

namespace CRM.Infrastructure.Identity;

public static class IdentitySeeder
{
    private static readonly string[] Roles = ["Admin", "Manager", "Staff"];

    // Chạy ở MỌI môi trường (không chỉ Development) — 3 role này là cấu trúc hệ thống,
    // không phải dữ liệu demo như Category/Product.
    public static async Task SeedRolesAsync(RoleManager<ApplicationRole> roleManager)
    {
        foreach (var role in Roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new ApplicationRole { Name = role });
            }
        }
    }

    // Chỉ Development — tạo sẵn 1 tài khoản Admin để có thể test ngay [Authorize(Roles=...)],
    // vì không có cách nào "tự phong" Admin đầu tiên qua API (mọi user đăng ký đều mặc định Staff).
    public static async Task SeedDevAdminAsync(UserManager<ApplicationUser> userManager)
    {
        const string adminEmail = "admin@crm.local";

        if (await userManager.FindByEmailAsync(adminEmail) is not null)
        {
            return;
        }

        var admin = new ApplicationUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            FullName = "Dev Admin",
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(admin, "Admin@12345");

        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(admin, "Admin");
        }
    }
}
