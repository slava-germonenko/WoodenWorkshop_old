namespace WoodenWorkshop.Crm.Api.Models.Http;

public record AuthorizationResponse(string AccessToken, string RefreshToken, string TokenType, int ExpiresIn)
{
    public string AccessToken { get; set; } = AccessToken;
    
    public string RefreshToken { get; set; } = RefreshToken;

    public string TokenType { get; set; } = TokenType;

    public int ExpiresIn { get; set; } = ExpiresIn;
}