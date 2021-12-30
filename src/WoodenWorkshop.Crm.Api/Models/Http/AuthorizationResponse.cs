namespace WoodenWorkshop.Crm.Api.Models.Http;

public record AuthorizationResponse(
    string AccessToken,
    string RefreshToken, 
    int ExpiresIn,
    string TokenType = "bearer"
);