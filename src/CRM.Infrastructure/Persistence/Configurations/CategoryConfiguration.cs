using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CRM.Domain.Entities;

namespace CRM.Infrastructure.Persistence.Configurations
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.Property(c => c.Name)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.HasIndex(c => c.Name)
                   .IsUnique();

            builder.Property(c => c.Description)
                   .HasMaxLength(500);

            // Quan hệ Category 1-n Product đã cấu hình ở ProductConfiguration (phía "many"),
            // không khai báo lại ở đây để tránh 1 quan hệ bị mô tả ở 2 nơi.
        }
    }
}
