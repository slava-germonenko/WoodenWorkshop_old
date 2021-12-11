using System.Text.Json.Serialization;

namespace WoodenWorkshop.Crm.Api.Models;

public record AuthorizationResponse(string AccessToken, string RefreshToken, string TokenType, int ExpiresIn)
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; } = AccessToken;

    [JsonPropertyName("refresh_token")]
    public string RefreshToken { get; set; } = RefreshToken;

    [JsonPropertyName("token_type")]
    public string TokenType { get; set; } = TokenType;

    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; set; } = ExpiresIn;
}