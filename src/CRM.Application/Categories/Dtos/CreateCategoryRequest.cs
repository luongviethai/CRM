namespace CRM.Application.Categories.Dtos;

public record CreateCategoryRequest(
    string Name,
    string? Description
);
