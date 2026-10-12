namespace CRM.Application.Products.Dtos;

public record ProductDto(
    int Id,
    string SKU,
    string Name,
    string? Description,
    decimal Price,
    int StockQuantity,
    string Unit,
    bool IsActive,
    int CategoryId,
    string CategoryName,
    int SupplierId,
    string SupplierName
);
