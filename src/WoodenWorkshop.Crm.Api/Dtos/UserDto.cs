using System.ComponentModel.DataAnnotations;

using WoodenWorkshop.Core.Models;

namespace WoodenWorkshop.Crm.Api.Dtos;

public record UserDto(
    [Required] Guid Id,
    [Required] [StringLength(100)] [EmailAddress] string EmailAddress,
    [Required] [StringLength(100)] string FirstName,
    [Required] [StringLength(100)] string LastName
)
{
    public static explicit operator UserDto(User user) => new(
        user.Id,
        user.EmailAddress,
        user.FirstName,
        user.LastName
    );
}
