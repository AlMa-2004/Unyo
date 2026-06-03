using System.ComponentModel.DataAnnotations;

namespace Unyo.Dtos;

public record LoginDto(
    [Required, EmailAddress] string Email,
    [Required] string Password);

public record RegisterDto(
    [Required, EmailAddress] string Email,
    [Required, MinLength(6)] string Password,
    [Required] string FullName);