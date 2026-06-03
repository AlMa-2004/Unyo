using System.ComponentModel.DataAnnotations;

namespace Unyo.Dtos;

public record CategoryDto(int Id, string Name);

public record CreateCategoryDto([Required, MinLength(3), MaxLength(50)] string Name);