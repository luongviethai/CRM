namespace CRM.Application.Suppliers.Dtos;

public record CreateSupplierRequest(
    string Name,
    string? ContactName,
    string? Email,
    string? Phone,
    string? Address
);
