namespace CRM.Domain.Entities
{
    public class User
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Email { get; set; }
        public required string PasswordHash { get; set; }
        public required string Role { get; set; }

        public ICollection<Order> CreatedOrders { get; set; } = new List<Order>();
    }
}