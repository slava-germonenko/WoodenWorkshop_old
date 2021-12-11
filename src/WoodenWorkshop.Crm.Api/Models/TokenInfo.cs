namespace WoodenWorkshop.Crm.Api.Models;

public record TokenInfo(
    string Token,
    DateTime IssueDate,
    DateTime ExpireDate
);
