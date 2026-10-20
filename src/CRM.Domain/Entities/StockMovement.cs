namespace CRM.Domain.Entities
{
    public class StockMovement
    {
        public int Id { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;

        public StockMovementType Type { get; set; }

        // Dương = kho tăng, âm = kho giảm — cộng dồn toàn bộ Quantity của 1 sản phẩm phải luôn
        // bằng đúng Product.StockQuantity hiện tại (dùng để đối chiếu/audit sau này).
        public int Quantity { get; set; }

        // Trỏ tới nguồn gốc gây ra thay đổi kho (OrderId khi Type = Export/hoàn kho), null khi
        // nhập/điều chỉnh thủ công không gắn với đơn hàng nào. Không khai báo FK thật vì nghĩa
        // của cột này đổi theo Type, không phải lúc nào cũng là OrderId.
        public int? Reference { get; set; }

        public DateTime CreatedAtUtc { get; set; }

        public string? Note { get; set; }
    }

    public enum StockMovementType
    {
        Import,
        Export,
        Adjustment
    }
}
