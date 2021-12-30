namespace WoodenWorkshop.Auth.Jobs.Settings;

public interface IExpireRefreshTokensSettings : ITimedBackgroundServiceSettings
{
    public int ProcessTokensLimit { get; }
}