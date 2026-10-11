using Microsoft.EntityFrameworkCore;
using CRM.Domain.Entities;

namespace CRM.Application.Common.Interfaces
{
    public interface IApplicationDbContext
    {
        public DbSet<Category> Categories { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<User> Users { get; set; }


        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}