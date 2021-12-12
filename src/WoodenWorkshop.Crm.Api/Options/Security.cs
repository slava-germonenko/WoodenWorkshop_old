namespace WoodenWorkshop.Crm.Api.Options;

public class Security
{
    public int AccessTokenTtlSeconds { get; set; }

    public string JwtSecret { get; set; }

    public int RefreshTokenTtlMinutes { get; set; }
}