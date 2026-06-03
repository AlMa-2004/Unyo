using Unyo.Models;
using Unyo.Dtos;

namespace Unyo.Mappings;

public static class CategoryMappings
{
    public static CategoryDto MapToDto(this Category category) =>
        category == null ? null! : new CategoryDto(category.Id, category.Name);

    public static Category MapToEntity(this CreateCategoryDto dto) =>
        dto == null ? null! : new Category { Name = dto.Name };
}