namespace CRM.Infrastructure.Identity;

// Hằng số tên permission — tránh gõ tay chuỗi "product.delete" rải rác khắp nơi (dễ sai, không
// bắt lỗi lúc compile). Đúng 19 permission trong ma trận ở docs/2-tai-lieu-nghiep-vu.md mục 6.
// Sẽ dùng lại ở Ngày 13 cho [Authorize(Policy = PermissionCatalog.ProductDelete)].
public static class PermissionCatalog
{
    public const string CategoryView = "category.view";
    public const string CategoryManage = "category.manage";

    public const string SupplierView = "supplier.view";
    public const string SupplierManage = "supplier.manage";

    public const string ProductView = "product.view";
    public const string ProductCreate = "product.create";
    public const string ProductUpdate = "product.update";
    public const string ProductDelete = "product.delete";

    public const string CustomerView = "customer.view";
    public const string CustomerManage = "customer.manage";

    public const string OrderView = "order.view";
    public const string OrderCreate = "order.create";
    public const string OrderCancel = "order.cancel";
    public const string OrderComplete = "order.complete";

    public const string StockMovementView = "stockmovement.view";
    public const string StockMovementManual = "stockmovement.manual";

    public const string RoleManage = "role.manage";
    public const string PermissionManage = "permission.manage";
    public const string UserManage = "user.manage";

    public static readonly string[] All =
    [
        CategoryView, CategoryManage,
        SupplierView, SupplierManage,
        ProductView, ProductCreate, ProductUpdate, ProductDelete,
        CustomerView, CustomerManage,
        OrderView, OrderCreate, OrderCancel, OrderComplete,
        StockMovementView, StockMovementManual,
        RoleManage, PermissionManage, UserManage
    ];
}
