using CRM.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CRM.Infrastructure.Identity;

public static class IdentitySeeder
{
    private static readonly string[] Roles = ["Admin", "Manager", "Staff"];

    // Đúng ma trận ở docs/2-tai-lieu-nghiep-vu.md mục 6 — Admin có mọi permission (không liệt kê
    // tay 19 dòng cho đỡ lặp/dễ sai khi matrix đổi), Manager/Staff liệt kê đúng tập được tick ✓.
    private static readonly Dictionary<string, string[]> RolePermissionMatrix = new()
    {
        ["Admin"] = PermissionCatalog.All,
        ["Manager"] =
        [
            PermissionCatalog.CategoryView, PermissionCatalog.CategoryManage,
            PermissionCatalog.SupplierView, PermissionCatalog.SupplierManage,
            PermissionCatalog.ProductView, PermissionCatalog.ProductCreate, PermissionCatalog.ProductUpdate,
            PermissionCatalog.CustomerView, PermissionCatalog.CustomerManage,
            PermissionCatalog.OrderView, PermissionCatalog.OrderCreate, PermissionCatalog.OrderCancel, PermissionCatalog.OrderComplete,
            PermissionCatalog.StockMovementView, PermissionCatalog.StockMovementManual
        ],
        ["Staff"] =
        [
            PermissionCatalog.CategoryView,
            PermissionCatalog.SupplierView,
            PermissionCatalog.ProductView,
            PermissionCatalog.CustomerView, PermissionCatalog.CustomerManage,
            PermissionCatalog.OrderView, PermissionCatalog.OrderCreate, PermissionCatalog.OrderComplete,
            PermissionCatalog.StockMovementView
        ]
    };

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

    // Cũng chạy ở mọi môi trường, cùng lý do với role — danh sách permission + ma trận gán cho
    // role là cấu trúc hệ thống, Production cần có y hệt Development.
    public static async Task SeedPermissionsAsync(AppDbContext context)
    {
        var existingNames = await context.Permissions.Select(p => p.Name).ToListAsync();
        var missingNames = PermissionCatalog.All.Except(existingNames);

        foreach (var name in missingNames)
        {
            context.Permissions.Add(new Permission { Name = name });
        }

        await context.SaveChangesAsync();
    }

    public static async Task SeedRolePermissionsAsync(AppDbContext context, RoleManager<ApplicationRole> roleManager)
    {
        var permissionIdsByName = await context.Permissions.ToDictionaryAsync(p => p.Name, p => p.Id);

        foreach (var (roleName, permissionNames) in RolePermissionMatrix)
        {
            var role = await roleManager.FindByNameAsync(roleName);
            if (role is null)
            {
                continue; // role chưa tồn tại — không nên xảy ra nếu gọi SeedRolesAsync trước
            }

            var alreadyAssigned = await context.RolePermissions
                .Where(rp => rp.RoleId == role.Id)
                .Select(rp => rp.PermissionId)
                .ToListAsync();

            foreach (var permissionName in permissionNames)
            {
                var permissionId = permissionIdsByName[permissionName];

                if (!alreadyAssigned.Contains(permissionId))
                {
                    context.RolePermissions.Add(new RolePermission { RoleId = role.Id, PermissionId = permissionId });
                }
            }
        }

        await context.SaveChangesAsync();
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
