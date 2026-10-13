namespace CRM.Application.Customers.Dtos;

public record CustomerDto(
    int Id,
    string Name,
    string? Email,
    string? Phone,
    string? Address
);
