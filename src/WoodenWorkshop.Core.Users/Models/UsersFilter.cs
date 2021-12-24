namespace WoodenWorkshop.Core.Users.Models;

public record UsersFilter(
    string? FirstName,
    string? LastName,
    string? EmailAddress,
    string? Search
);
