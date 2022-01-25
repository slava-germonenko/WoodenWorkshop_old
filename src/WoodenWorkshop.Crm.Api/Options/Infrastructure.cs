namespace WoodenWorkshop.Crm.Api.Options;

public class Infrastructure
{
    public string AuthSqlConnectionString { get; set; }
    
    public string CoreSqlConnectionString { get; set; }

    public int ExpireRefreshTokenIntervalMinutes { get; set; }
    
    public int ExpireRefreshTokenProcessLimit { get; set; }
}