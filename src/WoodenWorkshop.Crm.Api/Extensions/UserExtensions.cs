using WoodenWorkshop.Core.Models;
using WoodenWorkshop.Crm.Api.Dtos;

namespace WoodenWorkshop.Crm.Api.Extensions;

public static class UserExtensions
{
    public static void UpdateFromUserDto(this User user, UserDto userDto)
    {
        user.EmailAddress = userDto.EmailAddress;
        user.FirstName = userDto.FirstName;
        user.LastName = userDto.LastName;
    }
}