namespace WoodenWorkshop.Crm.Api.Models.Http;

public record AuthorizationResponse(string AccessToken, string RefreshToken, string TokenType, int ExpiresIn);