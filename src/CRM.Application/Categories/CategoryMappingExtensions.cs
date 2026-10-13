using CRM.Application.Categories.Dtos;
using CRM.Domain.Entities;

namespace CRM.Application.Categories;

public static class CategoryMappingExtensions
{
    public static CategoryDto ToDto(this Category category)
    {
        return new CategoryDto(
            category.Id,
            category.Name,
            category.Description
        );
    }

    public static Category ToEntity(this CreateCategoryRequest request)
    {
        return new Category
        {
            Name = request.Name,
            Description = request.Description
        };
    }

    public static void ApplyTo(this UpdateCategoryRequest request, Category category)
    {
        category.Name = request.Name;
        category.Description = request.Description;
    }
}
