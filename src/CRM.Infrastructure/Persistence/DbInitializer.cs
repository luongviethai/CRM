using CRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CRM.Infrastructure.Persistence
{
    // Seed dữ liệu test cho local dev — chỉ gọi khi IsDevelopment() (xem Program.cs).
    // Không dùng HasData vì cần gán quan hệ qua navigation property cho tiện,
    // không muốn tự tính tay Id như HasData yêu cầu.
    public static class DbInitializer
    {
        public static async Task SeedAsync(AppDbContext context)
        {
            if (await context.Categories.AnyAsync())
            {
                return; // đã có dữ liệu rồi, không seed lại
            }

            var dienTu = new Category { Name = "Điện tử", Description = "Thiết bị điện tử, linh kiện, phụ kiện" };
            var vanPhongPham = new Category { Name = "Văn phòng phẩm", Description = "Dụng cụ, vật tư văn phòng" };
            var doGiaDung = new Category { Name = "Đồ gia dụng", Description = "Thiết bị, dụng cụ gia dụng" };

            var supplierAbc = new Supplier
            {
                Name = "Công ty TNHH Thương mại ABC",
                ContactName = "Nguyễn Văn An",
                Email = "contact@abc-trading.vn",
                Phone = "0901234567",
                Address = "123 Lê Lợi, Quận 1, TP.HCM"
            };
            var supplierXyz = new Supplier
            {
                Name = "Công ty CP Phân phối XYZ",
                ContactName = "Trần Thị Bình",
                Email = "sales@xyz-distribution.vn",
                Phone = "0912345678",
                Address = "456 Nguyễn Huệ, Quận 1, TP.HCM"
            };

            var customers = new List<Customer>
            {
                new() { Name = "Lê Văn Cường", Email = "levancuong@gmail.com", Phone = "0987654321", Address = "789 Hai Bà Trưng, Quận 3, TP.HCM" },
                new() { Name = "Phạm Thị Dung", Email = "phamthidung@gmail.com", Phone = "0976543210", Address = "321 Võ Văn Tần, Quận 3, TP.HCM" },
                new() { Name = "Công ty TNHH Giải pháp Số", Email = "procurement@gpso.vn", Phone = "0965432109", Address = "654 Cách Mạng Tháng 8, Quận 10, TP.HCM" },
            };

            context.Categories.AddRange(dienTu, vanPhongPham, doGiaDung);
            context.Suppliers.AddRange(supplierAbc, supplierXyz);
            context.Customers.AddRange(customers);

            // Save trước để Category/Supplier có Id thật, rồi mới tạo Product gán navigation.
            await context.SaveChangesAsync();

            var products = new List<Product>
            {
                new()
                {
                    SKU = "DT-001", Name = "Chuột không dây Logitech M331", Description = "Chuột không dây, click êm",
                    Price = 350000m, StockQuantity = 50, Unit = "cái", IsActive = true,
                    Category = dienTu, Supplier = supplierAbc
                },
                new()
                {
                    SKU = "DT-002", Name = "Bàn phím cơ AKKO 3068", Description = "Bàn phím cơ 68 phím, switch đỏ",
                    Price = 890000m, StockQuantity = 30, Unit = "cái", IsActive = true,
                    Category = dienTu, Supplier = supplierAbc
                },
                new()
                {
                    SKU = "DT-003", Name = "Tai nghe Bluetooth Sony WH-CH520", Description = "Tai nghe chụp tai, pin 50 giờ",
                    Price = 1250000m, StockQuantity = 20, Unit = "cái", IsActive = true,
                    Category = dienTu, Supplier = supplierXyz
                },
                new()
                {
                    SKU = "VPP-001", Name = "Giấy A4 Double A 70gsm", Description = "Ram giấy A4, 500 tờ",
                    Price = 75000m, StockQuantity = 200, Unit = "ram", IsActive = true,
                    Category = vanPhongPham, Supplier = supplierXyz
                },
                new()
                {
                    SKU = "VPP-002", Name = "Bút bi Thiên Long TL-027", Description = "Hộp 10 cây bút bi mực xanh",
                    Price = 25000m, StockQuantity = 150, Unit = "hộp", IsActive = true,
                    Category = vanPhongPham, Supplier = supplierAbc
                },
                new()
                {
                    SKU = "GD-001", Name = "Quạt điện Panasonic F-409KH", Description = "Quạt đứng 3 tốc độ",
                    Price = 650000m, StockQuantity = 15, Unit = "cái", IsActive = true,
                    Category = doGiaDung, Supplier = supplierXyz
                },
                new()
                {
                    SKU = "GD-002", Name = "Nồi cơm điện Sharp KS-COMA", Description = "Nồi cơm điện 1.8L",
                    Price = 1100000m, StockQuantity = 10, Unit = "cái", IsActive = true,
                    Category = doGiaDung, Supplier = supplierAbc
                },
            };

            context.Products.AddRange(products);
            await context.SaveChangesAsync();
        }
    }
}
