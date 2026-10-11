
namespace CRM.Domain.Entities
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string SKU { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public int CategoryId { get; set; }
        public int SupplierId { get; set; }
        public bool IsActive { get; set; } = true;
        public string Unit { get; set; }
        public Category Category { get; set; }
        public Supplier Supplier { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}