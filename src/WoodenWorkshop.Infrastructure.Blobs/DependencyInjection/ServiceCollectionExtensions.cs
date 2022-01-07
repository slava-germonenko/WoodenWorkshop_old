using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using WoodenWorkshop.Infrastructure.Blobs.Abstractions;

namespace WoodenWorkshop.Infrastructure.Blobs.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static void AddBlobServiceFactory(
        this IServiceCollection services,
        BlobServiceFactoryOptions options,
        ServiceLifetime factoryLifetime = ServiceLifetime.Scoped,
        ServiceLifetime optionsLifetime = ServiceLifetime.Singleton
    )
    {
        services.TryAdd(
            new ServiceDescriptor(
                typeof(IBlobServiceFactoryOptions),
                (p) => options, 
                optionsLifetime
            )
        );
        
        services.TryAdd(
            new ServiceDescriptor(
                typeof(IBlobServiceFactory),
                (p) => new BlobServiceFactory(
                    p.GetRequiredService<IBlobServiceFactoryOptions>()
                ), 
                factoryLifetime
            )
        );
    }

    public static void AddBlobServiceFactory(this IServiceCollection services, string connectionString)
    {
        services.AddBlobServiceFactory(new BlobServiceFactoryOptions
        {
            ConnectionString = connectionString
        });
    }
}