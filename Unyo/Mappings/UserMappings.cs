using Unyo.Models;
using Unyo.Dtos;

namespace Unyo.Mappings;

public static class UserMappings
{
    public static UserDto MapToDto(this User user) =>
        user == null ? null! : new UserDto(user.Id, user.UserName ?? string.Empty, user.Email ?? string.Empty, user.FirstName, user.LastName, user.BirthDate ?? DateTime.MinValue);
}