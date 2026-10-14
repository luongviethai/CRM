namespace CRM.Domain.Entities
{
    public class Order
    {
        public int Id { get; set; }
        public required string OrderCode { get; set; }

        public int CustomerId { get; set; }
        public Customer Customer { get; set; } = null!;
        public DateTime OrderDate { get; set; }

        public decimal TotalAmount { get; set; }

        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        public string? Notes { get; set; }

        // Id của user đã tạo đơn — chỉ scalar FK, không có navigation property vì
        // user thật (ApplicationUser) sống ở Infrastructure (ASP.NET Identity), Domain
        // không được phụ thuộc Infrastructure. Quan hệ được cấu hình ở OrderConfiguration.
        public int CreatedBy { get; set; }

        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }

    public enum OrderStatus
    {
        Pending,
        Processing,
        Completed,
        Cancelled
    }
}

