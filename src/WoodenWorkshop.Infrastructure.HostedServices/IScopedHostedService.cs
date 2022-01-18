namespace WoodenWorkshop.Infrastructure.HostedServices;

public interface IScopedHostedService
{
    Task ExecuteAsync(CancellationToken cancellationToken);
}