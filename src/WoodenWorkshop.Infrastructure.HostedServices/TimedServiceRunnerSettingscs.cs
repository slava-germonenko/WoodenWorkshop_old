namespace WoodenWorkshop.Infrastructure.HostedServices;

public class TimedServiceRunnerSettings<TService> : ITimedServiceRunnerSettings<TService>
{
    public TimeSpan SleepTime { get; set; }

    public TimedServiceRunnerSettings(TimeSpan sleepTime)
    {
        SleepTime = sleepTime;
    }
}