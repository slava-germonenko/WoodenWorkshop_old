using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using WoodenWorkshop.Auth;
using WoodenWorkshop.Auth.Services;
using WoodenWorkshop.Auth.Services.Abstractions;
using WoodenWorkshop.Core;
using WoodenWorkshop.Core.Assets.Services;
using WoodenWorkshop.Core.Assets.Services.Abstractions;

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

                builder.Services.AddScoped<IAssetsCleanupService, AssetsCleanupService>();
                builder.Services.AddScoped<IAssetsService, AssetsService>();
                builder.Services.AddScoped<IFoldersCleanupService, FoldersCleanupService>();
                builder.Services.AddScoped<IFoldersService, FoldersService>();
                builder.Services.AddScoped<ISessionsCleanupService, SessionsCleanupService>();

                var blobStorageConnectionString = configuration.GetValue<string>("Infrastructure:BlobStorageConnectionString");
                var serviceBusConnectionString = configuration.GetValue<string>("Infrastructure:ServiceBusConnectionString");
                builder.Services.AddAzureClients(azureBuilder =>
                {
                    azureBuilder.AddBlobServiceClient(blobStorageConnectionString);
                    azureBuilder.AddServiceBusClient(serviceBusConnectionString);
                });

                var coreContextConnectionString = configuration
                    .GetValue<string>("Infrastructure:CoreSqlConnectionString");
                builder.Services.AddDbContext<CoreContext>(options =>
                {
                    options.UseSqlServer(coreContextConnectionString);
                });

                var authContextConnectionString = configuration
                    .GetValue<string>("Infrastructure:AuthSqlConnectionString");
                builder.Services.AddDbContext<AuthContext>(options =>
                {
                    options.UseSqlServer(authContextConnectionString);
                });

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