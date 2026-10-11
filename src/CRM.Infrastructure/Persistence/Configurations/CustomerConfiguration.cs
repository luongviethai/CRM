using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CRM.Domain.Entities;

namespace CRM.Infrastructure.Persistence.Configurations
{
    public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.Property(c => c.Name)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(c => c.Email)
                   .HasMaxLength(256);

            builder.Property(c => c.Phone)
                   .HasMaxLength(20);

            builder.Property(c => c.Address)
                   .HasMaxLength(500);

            // Quan hệ Customer 1-n Order cấu hình ở OrderConfiguration (phía "many",
            // vì Order là bên giữ CustomerId).
        }
    }
}
