using System.ComponentModel.DataAnnotations;

namespace Unyo.Dtos;

public record UserDto(string Id, string UserName, string Email, string FirstName, string LastName, DateTime BirthDate);

public record CreateUserDto(
    [Required, EmailAddress] string Email,
    [Required, MinLength(2)] string FirstName,
    [Required, MinLength(2)] string LastName,
    DateTime BirthDate);

public record UpdateUserDto(
    [Required, EmailAddress] string Email,
    [Required, MinLength(2)] string FirstName,
    [Required, MinLength(2)] string LastName,
    DateTime BirthDate);