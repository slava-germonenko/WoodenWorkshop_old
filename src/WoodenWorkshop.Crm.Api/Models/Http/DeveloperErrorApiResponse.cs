namespace WoodenWorkshop.Crm.Api.Models.Http;

public class DeveloperErrorApiResponse : BaseApiResponse
{
    public string? Stacktrace { get; }

    public DeveloperErrorApiResponse(string message, string? stacktrace) : base(message)
    {
        Stacktrace = stacktrace;
    }
}