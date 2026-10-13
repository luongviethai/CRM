namespace CRM.Application.Products.Dtos;

public record ProductQueryParams(
    int Page = 1,
    int PageSize = 10,
    int? CategoryId = null,
    string? Name = null,
    string? SortBy = null,
    bool SortDescending = false
);