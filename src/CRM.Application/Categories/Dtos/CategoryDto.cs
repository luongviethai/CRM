namespace CRM.Application.Categories.Dtos;

public record CategoryDto(
    int Id,
    string Name,
    string? Description
);
