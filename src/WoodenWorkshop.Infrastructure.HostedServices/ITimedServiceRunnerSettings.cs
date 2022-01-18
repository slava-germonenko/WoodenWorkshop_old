namespace WoodenWorkshop.Infrastructure.HostedServices;

public interface ITimedServiceRunnerSettings<TService>
{
    public TimeSpan SleepTime { get; }
}