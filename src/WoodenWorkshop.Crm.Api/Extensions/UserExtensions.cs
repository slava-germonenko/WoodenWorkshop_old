using WoodenWorkshop.Core.Models;
using WoodenWorkshop.Crm.Api.Dtos;

namespace WoodenWorkshop.Crm.Api.Extensions;

public static class UserExtensions
{
    public static void UpdateFromProfile(this User user, ProfileDto profile)
    {
        user.EmailAddress = profile.EmailAddress;
        user.FirstName = profile.FirstName;
        user.LastName = profile.LastName;
    }
}