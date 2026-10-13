namespace CRM.Application.Suppliers.Dtos;

public record SupplierDto(
    int Id,
    string Name,
    string? ContactName,
    string? Email,
    string? Phone,
    string? Address
);
