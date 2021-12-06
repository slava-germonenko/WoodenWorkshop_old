namespace WoodenWorkshop.Crm.Api.Models;

public class BaseApiResponse
{
    public string Message { get; set; }

    public BaseApiResponse(string message)
    {
        Message = message;
    }
}
