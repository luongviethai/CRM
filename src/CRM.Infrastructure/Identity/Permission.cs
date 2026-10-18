namespace CRM.Infrastructure.Identity;

public class Permission
{
    public int Id { get; set; }

    // Dạng "resource.action" (product.delete, order.cancel...) — xem ma trận đầy đủ ở
    // docs/2-tai-lieu-nghiep-vu.md mục 6.
    public required string Name { get; set; }

    public string? Description { get; set; }
}
