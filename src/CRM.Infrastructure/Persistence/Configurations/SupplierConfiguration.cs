using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CRM.Domain.Entities;

namespace CRM.Infrastructure.Persistence.Configurations
{
    public class SupplierConfiguration : IEntityTypeConfiguration<Supplier>
    {
        public void Configure(EntityTypeBuilder<Supplier> builder)
        {
            builder.Property(s => s.Name)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(s => s.ContactName)
                   .HasMaxLength(200);

            builder.Property(s => s.Email)
                   .HasMaxLength(256);

            builder.Property(s => s.Phone)
                   .HasMaxLength(20);

            builder.Property(s => s.Address)
                   .HasMaxLength(500);

            // Quan hệ Supplier 1-n Product đã cấu hình ở ProductConfiguration (phía "many").
        }
    }
}
