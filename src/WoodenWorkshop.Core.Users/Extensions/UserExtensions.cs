using WoodenWorkshop.Core.Models;

namespace WoodenWorkshop.Core.Users.Extensions;

public static class UserExtensions
{
    public static void CopyDetails(this User target, User source)
    {
        target.EmailAddress = source.EmailAddress;
        target.FirstName = source.FirstName;
        target.LastName = source.LastName;
    }
}