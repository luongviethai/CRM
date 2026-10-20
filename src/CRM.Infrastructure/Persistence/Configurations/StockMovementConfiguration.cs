using CRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM.Infrastructure.Persistence.Configurations
{
    public class StockMovementConfiguration : IEntityTypeConfiguration<StockMovement>
    {
        public void Configure(EntityTypeBuilder<StockMovement> builder)
        {
            builder.Property(m => m.Note)
                   .HasMaxLength(500);

            builder.HasOne(m => m.Product)
                   .WithMany(p => p.StockMovements)
                   .HasForeignKey(m => m.ProductId)
                   .OnDelete(DeleteBehavior.Restrict); // giữ lịch sử kho, không xóa kéo theo khi Product bị xóa

            builder.HasIndex(m => m.ProductId); // tra lịch sử kho theo sản phẩm (API Ngày 18)
        }
    }
}
