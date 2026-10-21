using CRM.Application.Common.Models;
using CRM.Application.StockMovements.Dtos;

namespace CRM.Application.StockMovements;

public interface IStockMovementService
{
    // null = Product không tồn tại (404), Succeeded=false = vi phạm business rule (400/409).
    Task<StockMovementResult?> ImportAsync(int productId, ImportStockRequest request);
    Task<StockMovementResult?> AdjustAsync(int productId, AdjustStockRequest request);
    Task<PageResult<StockMovementDto>?> GetHistoryAsync(int productId, StockMovementQueryParams queryParams);
}
