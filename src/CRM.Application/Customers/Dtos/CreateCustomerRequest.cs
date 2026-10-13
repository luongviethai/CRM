namespace CRM.Application.Customers.Dtos;

public record CreateCustomerRequest(
    string Name,
    string? Email,
    string? Phone,
    string? Address
);
