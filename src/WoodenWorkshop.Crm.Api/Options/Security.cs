namespace WoodenWorkshop.Crm.Api.Options;

public class Security
{
    public string JwtSecret { get; set; }

    public int AccessTokenTtlSeconds { get; set; }

    public int RefreshTokenTtlMinutes { get; set; }
}