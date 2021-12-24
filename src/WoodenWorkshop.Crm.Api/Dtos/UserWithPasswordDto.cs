using System.ComponentModel.DataAnnotations;

using WoodenWorkshop.Common.Hashing;
using WoodenWorkshop.Core.Models;

namespace WoodenWorkshop.Crm.Api.Dtos;

public record UserWithPasswordDto(
    [Required] Guid? Id,
    [Required] [StringLength(100)] [EmailAddress] string EmailAddress,
    [Required] [StringLength(100)] string FirstName,
    [Required] [StringLength(100)] string LastName,
    [Required] string Password
)
{
    public static explicit operator User(UserWithPasswordDto userDto)
    {
        using var hashingUtility = new HashingUtility();
        return new()
        {
            Id = userDto.Id ?? Guid.Empty,
            FirstName = userDto.FirstName,
            LastName = userDto.LastName,
            EmailAddress = userDto.EmailAddress,
            PasswordHash = hashingUtility.ComputeHash(userDto.Password)
        };
    }
}
