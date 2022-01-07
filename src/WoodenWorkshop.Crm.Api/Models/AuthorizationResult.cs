using WoodenWorkshop.Auth.Models;

namespace WoodenWorkshop.Crm.Api.Models;

public record AuthorizationResult(
    TokenInfo AccessToken,
    TokenInfo RefreshToken,
    Session Session
);