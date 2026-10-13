namespace CRM.Application.Customers.Dtos;

public record UpdateCustomerRequest(
    string Name,
    string? Email,
    string? Phone,
    string? Address
);
