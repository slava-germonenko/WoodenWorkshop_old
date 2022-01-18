namespace WoodenWorkshop.Auth.HostedServices.Settings;

public interface IExpireTokenServiceSettings
{
    public int ExpireRefreshTokenProcessLimit { get; }
}