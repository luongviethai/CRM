namespace CRM.Application.Categories.Dtos;

public record UpdateCategoryRequest(
    string Name,
    string? Description
);
