namespace CRM.Application.StockMovements.Dtos;

// Quantity được âm (kiểm kê phát hiện thiếu) hoặc dương (phát hiện thừa), chỉ cấm bằng 0.
public record AdjustStockRequest(int Quantity, string? Note);
