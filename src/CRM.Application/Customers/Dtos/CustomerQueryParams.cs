namespace CRM.Application.Customers.Dtos;

public record CustomerQueryParams(
    int Page = 1,
    int PageSize = 10,
    string? Name = null,
    string? SortBy = null,
    bool SortDescending = false
);
