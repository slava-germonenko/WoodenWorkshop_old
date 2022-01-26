using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using WoodenWorkshop.Core;
using WoodenWorkshop.Core.Assets.Services;
using WoodenWorkshop.Core.Assets.Services.Abstractions;
using WoodenWorkshop.Crm.Functions.Middleware;

namespace WoodenWorkshop.Crm.Functions;

public static class Program
{
    public static void Main()
    {
        var host = new HostBuilder()
            .ConfigureAppConfiguration((_, builder) =>
            {
                builder.AddConfiguration(BuildConfiguration());
            })
            .ConfigureFunctionsWorkerDefaults((_, builder) =>
            {
                var configuration = BuildConfiguration();
                
                builder.Services.AddScoped<IAssetsService, AssetsService>();

                var blobStorageConnectionString = configuration.GetValue<string>("Infrastructure:BlobStorageConnectionString");
                builder.Services.AddAzureClients(azureBuilder =>
                {
                    azureBuilder.AddBlobServiceClient(blobStorageConnectionString);
                });

                var coreContextConnectionString = configuration.GetValue<string>("Infrastructure:CoreSqlConnectionString");
                builder.Services.AddDbContext<CoreContext>(options =>
                {
                    options.UseSqlServer(coreContextConnectionString);
                });

                builder.UseMiddleware<ExceptionHandlingMiddleware>();
            })
            .Build();

        host.Run();
    }

    private static IConfiguration BuildConfiguration()
    {
        var configurationBuilder = new ConfigurationBuilder();
        configurationBuilder.AddEnvironmentVariables();

        var envVariablesConfig = configurationBuilder.Build();
        var appConfigurationConnectionString = envVariablesConfig.GetValue<string>(
            "Infrastructure:AppConfigurationConnectionString"
        );

        if (string.IsNullOrEmpty(appConfigurationConnectionString))
        {
            configurationBuilder.AddJsonFile("local.settings.json");
        }
        else
        {
            configurationBuilder.AddAzureAppConfiguration(appConfigurationConnectionString);
        }

        var config =  configurationBuilder.Build();
        return config;
    }
}