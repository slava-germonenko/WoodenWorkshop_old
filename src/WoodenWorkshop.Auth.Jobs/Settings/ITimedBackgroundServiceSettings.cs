namespace WoodenWorkshop.Auth.Jobs.Settings;

public interface ITimedBackgroundServiceSettings
{
    TimeSpan SleepTime { get; }
}