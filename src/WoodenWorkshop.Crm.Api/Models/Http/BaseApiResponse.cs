namespace WoodenWorkshop.Crm.Api.Models.Http;

public class BaseApiResponse
{
    public string Message { get; set; }

    public BaseApiResponse(string message)
    {
        Message = message;
    }
}
