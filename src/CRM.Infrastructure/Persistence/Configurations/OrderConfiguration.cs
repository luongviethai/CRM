using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CRM.Domain.Entities;
using CRM.Infrastructure.Identity;

namespace CRM.Infrastructure.Persistence.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.Property(o => o.OrderCode)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.HasIndex(o => o.OrderCode)
                   .IsUnique();

            builder.Property(o => o.Notes)
                   .HasMaxLength(1000);

            builder.Property(o => o.TotalAmount)
                   .HasPrecision(18, 2);

            builder.HasOne(o => o.Customer)
                   .WithMany(c => c.Orders)
                   .HasForeignKey(o => o.CustomerId)
                   .OnDelete(DeleteBehavior.Restrict); // không xóa Customer còn lịch sử đơn hàng

            // Order không có navigation property sang user (xem ghi chú ở Order.cs) — cấu hình
            // quan hệ 1 chiều tới ApplicationUser (AspNetUsers) chỉ qua FK, không qua CLR property.
            builder.HasOne<ApplicationUser>()
                   .WithMany()
                   .HasForeignKey(o => o.CreatedBy)
                   .OnDelete(DeleteBehavior.Restrict); // giữ truy vết ai tạo đơn, không xóa User kéo theo mất dữ liệu

            // Quan hệ Order 1-n OrderItem cấu hình ở OrderItemConfiguration (phía "many").
        }
    }
}
