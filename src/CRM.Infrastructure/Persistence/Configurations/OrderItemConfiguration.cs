using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CRM.Domain.Entities;

namespace CRM.Infrastructure.Persistence.Configurations
{
    public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            builder.Property(oi => oi.UnitPrice)
                   .HasPrecision(18, 2);

            // TotalPrice là giá trị tính toán (Quantity * UnitPrice), không lưu cột riêng trong DB.
            builder.Ignore(oi => oi.TotalPrice);

            builder.HasOne(oi => oi.Order)
                   .WithMany(o => o.OrderItems)
                   .HasForeignKey(oi => oi.OrderId)
                   .OnDelete(DeleteBehavior.Cascade); // xóa Order thì xóa luôn OrderItem của nó

            // Quan hệ OrderItem - Product đã cấu hình đầy đủ ở ProductConfiguration
            // (HasMany(p => p.OrderItems)...), không khai báo lại ở đây.
        }
    }
}
