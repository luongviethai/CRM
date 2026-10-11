namespace CRM.Domain.Entities
{
    public class Order
    {
        public int Id { get; set; }
        public string OrderCode { get; set; }

        public int CustomerId { get; set; }
        public Customer Customer { get; set; }
        public DateTime OrderDate { get; set; }

        public decimal TotalAmount { get; set; }

        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        public string Notes { get; set; }

        public int CreatedBy { get; set; }
        public User CreatedByUser { get; set; }

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

