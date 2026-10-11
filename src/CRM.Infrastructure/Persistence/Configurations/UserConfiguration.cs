using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CRM.Domain.Entities;

namespace CRM.Infrastructure.Persistence.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.Property(u => u.Name)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(u => u.Email)
                   .IsRequired()
                   .HasMaxLength(256);

            builder.HasIndex(u => u.Email)
                   .IsUnique();

            builder.Property(u => u.PasswordHash)
                   .IsRequired();

            builder.Property(u => u.Role)
                   .IsRequired()
                   .HasMaxLength(50);

            // Quan hệ User 1-n Order (CreatedBy) cấu hình ở OrderConfiguration (phía "many").
            // Lưu ý: User này là entity tạm cho Tuần 1 — sẽ xem lại khi tích hợp ASP.NET Identity ở Tuần 2.
        }
    }
}
