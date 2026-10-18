namespace CRM.Infrastructure.Identity;

// Bảng nối n-n giữa ApplicationRole và Permission — dùng entity nối tường minh (không dùng
// skip-navigation ẩn của EF Core) để seed/query trực tiếp cho dễ, không cần load cả 2 entity cha.
public class RolePermission
{
    public int RoleId { get; set; }
    public ApplicationRole Role { get; set; } = null!;

    public int PermissionId { get; set; }
    public Permission Permission { get; set; } = null!;
}
