using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CRM.Domain.Entities;

namespace CRM.Infrastructure.Persistence.Configurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {

        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.Property(p => p.SKU)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.HasIndex(p => p.SKU)
                   .IsUnique();

            builder.Property(p => p.Name)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(p => p.Description)
                   .HasMaxLength(1000);

            builder.Property(p => p.Unit)
                   .HasMaxLength(20);

            builder.Property(p => p.Price)
                   .HasPrecision(18, 2);

            builder.HasOne(p => p.Category)
                    .WithMany(c => c.Products)
                   .HasForeignKey(p => p.CategoryId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.Supplier)
            .WithMany(s => s.Products)
              .HasForeignKey(p => p.SupplierId)
              .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(p => p.OrderItems)
                   .WithOne(oi => oi.Product)
                   .HasForeignKey(oi => oi.ProductId)
                   .OnDelete(DeleteBehavior.Restrict);

            // xmin là cột hệ thống có sẵn của Postgres, tự tăng mỗi lần UPDATE — không tạo cột
            // mới, chỉ khai báo cho EF Core dùng nó làm concurrency token (shadow property, không
            // có trên Product.cs, giữ Domain sạch không biết tới khái niệm đặc thù Postgres).
            // Đặt tên đúng "xmin" (chữ thường) vì Npgsql dò theo tên này để map vào cột hệ thống.
            builder.Property<uint>("xmin")
                   .IsRowVersion();
        }
    }

}