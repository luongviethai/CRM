namespace CRM.Application.Suppliers.Dtos;

public record UpdateSupplierRequest(
    string Name,
    string? ContactName,
    string? Email,
    string? Phone,
    string? Address
);
