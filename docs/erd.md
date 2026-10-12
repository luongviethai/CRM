# ERD — Project 1: Quản lý bán hàng và kho

> Phản ánh đúng schema hiện có trong code (Ngày 1-5). Chưa có `StockMovement`, `Role`/`Permission`, `RefreshToken` — các bảng đó sẽ thêm ở Tuần 2-3, ERD sẽ cập nhật lại khi đó.

```mermaid
erDiagram
    CATEGORY ||--o{ PRODUCT : "1-n, Restrict"
    SUPPLIER ||--o{ PRODUCT : "1-n, Restrict"
    PRODUCT ||--o{ ORDER_ITEM : "1-n, Restrict"
    CUSTOMER ||--o{ ORDER : "1-n, Restrict"
    ORDER ||--o{ ORDER_ITEM : "1-n, Cascade"
    USER ||--o{ ORDER : "1-n (CreatedBy), Restrict"

    CATEGORY {
        int Id PK
        string Name "required, unique, maxlength 100"
        string Description "nullable, maxlength 500"
    }

    SUPPLIER {
        int Id PK
        string Name "required, maxlength 200"
        string ContactName "nullable"
        string Email "nullable, maxlength 256"
        string Phone "nullable, maxlength 20"
        string Address "nullable, maxlength 500"
    }

    PRODUCT {
        int Id PK
        string SKU UK "required, unique, maxlength 50"
        string Name "required, maxlength 200"
        string Description "nullable, maxlength 1000"
        decimal Price "precision 18,2"
        int StockQuantity
        string Unit "required, maxlength 20"
        bool IsActive
        int CategoryId FK
        int SupplierId FK
    }

    CUSTOMER {
        int Id PK
        string Name "required, maxlength 200"
        string Email "nullable, maxlength 256"
        string Phone "nullable, maxlength 20"
        string Address "nullable, maxlength 500"
    }

    ORDER {
        int Id PK
        string OrderCode UK "required, unique, maxlength 50"
        int CustomerId FK
        datetime OrderDate
        decimal TotalAmount "precision 18,2"
        int Status "enum: Pending/Processing/Completed/Cancelled"
        string Notes "nullable, maxlength 1000"
        int CreatedBy FK
    }

    ORDER_ITEM {
        int Id PK
        int OrderId FK
        int ProductId FK
        int Quantity
        decimal UnitPrice "precision 18,2"
    }

    USER {
        int Id PK
        string Name "required"
        string Email UK "required, unique"
        string PasswordHash "required"
        string Role "required — entity tạm, sẽ thay bằng ASP.NET Identity ở Tuần 2"
    }
```

## Ghi chú delete behavior (vì sao chọn Restrict/Cascade)

| Quan hệ | Behavior | Lý do |
|---|---|---|
| Category → Product | Restrict | Không xóa được danh mục còn sản phẩm — tránh mất liên kết dữ liệu |
| Supplier → Product | Restrict | Tương tự Category |
| Product → OrderItem | Restrict | Không xóa được sản phẩm đã từng xuất hiện trong đơn hàng — giữ lịch sử bán hàng |
| Customer → Order | Restrict | Không xóa khách hàng còn lịch sử đơn hàng |
| User → Order (CreatedBy) | Restrict | Giữ truy vết ai tạo đơn, không xóa user kéo theo mất dữ liệu đơn |
| Order → OrderItem | Cascade | OrderItem không có ý nghĩa tồn tại độc lập — xóa Order thì xóa luôn item của nó |

Chi tiết quy tắc nghiệp vụ đầy đủ xem [2-tai-lieu-nghiep-vu.md](2-tai-lieu-nghiep-vu.md).
