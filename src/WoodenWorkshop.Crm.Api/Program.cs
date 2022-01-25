using System.Text;
using Azure.Messaging.ServiceBus;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;
using Microsoft.IdentityModel.Tokens;

using WoodenWorkshop.Auth;
using WoodenWorkshop.Auth.HostedServices;
using WoodenWorkshop.Auth.HostedServices.Settings;
using WoodenWorkshop.Auth.Services;
using WoodenWorkshop.Auth.Services.Abstractions;
using WoodenWorkshop.Core;
using WoodenWorkshop.Core.Assets.Services;
using WoodenWorkshop.Core.Assets.Services.Abstractions;
using WoodenWorkshop.Core.Contacts.Services;
using WoodenWorkshop.Core.Contacts.Services.Abstractions;
using WoodenWorkshop.Core.Roles.Services;
using WoodenWorkshop.Core.Roles.Services.Abstractions;
using WoodenWorkshop.Core.Users.Services;
using WoodenWorkshop.Core.Users.Services.Abstractions;
using WoodenWorkshop.Crm.Api.Middleware;
using WoodenWorkshop.Crm.Api.Options;
using WoodenWorkshop.Crm.Api.Services;
using WoodenWorkshop.Crm.Api.Services.Abstractions;
using WoodenWorkshop.Crm.Api.Settings;
using WoodenWorkshop.Infrastructure.Blobs.DependencyInjection;
using WoodenWorkshop.Infrastructure.HostedServices.Extensions;

var builder = WebApplication.CreateBuilder(args);
var azureAppConfigurationConnectionString = builder.Configuration.GetValue<string>(
    "Infrastructure:AppConfigurationConnectionString"
);
if (!string.IsNullOrEmpty(azureAppConfigurationConnectionString))
{
    builder.Configuration.AddAzureAppConfiguration(azureAppConfigurationConnectionString);
}

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var jwtSecret = builder.Configuration.GetValue<string>("Security:JwtSecret");
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            RequireExpirationTime = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
            ClockSkew = TimeSpan.Zero,
        };
    });

builder.Services.Configure<Infrastructure>(builder.Configuration.GetSection("Infrastructure"));
builder.Services.Configure<Security>(builder.Configuration.GetSection("Security"));

// Services
builder.Services.AddScoped<IAssetsBulkActionsService, AssetsBulkActionsService>();
builder.Services.AddScoped<IAssetsCleanupService, AssetsCleanupService>();
builder.Services.AddScoped<IAssetsService, AssetsService>();
builder.Services.AddScoped<IAuthorizationService, AuthorizationService>();
builder.Services.AddScoped<IContactsListService, ContactsListService>();
builder.Services.AddScoped<IContactsService, ContactsService>();
builder.Services.AddScoped<IFoldersBulkActionsService, FoldersBulkActionsService>();
builder.Services.AddScoped<IFoldersCleanupService, FoldersCleanupService>();
builder.Services.AddScoped<IFoldersService, FoldersService>();
builder.Services.AddScoped<IRolePermissionsService, RolePermissionsService>();
builder.Services.AddScoped<IRolesListService, RolesListService>();
builder.Services.AddScoped<IRolesService, RolesService>();
builder.Services.AddScoped<IUserRolesService, UserRolesService>();
builder.Services.AddScoped<IUsersListService, UsersListService>();
builder.Services.AddScoped<IUsersService, UsersService>();
builder.Services.AddScoped<IUserSessionService, UserSessionService>();
builder.Services.AddScoped<ITokenService, JwtService>();

// Infrastructure
var authConnectionString = builder.Configuration.GetValue<string>("Infrastructure:AuthSqlConnectionString");
builder.Services.AddDbContext<AuthContext>(
    options => options.UseSqlServer(authConnectionString),
    optionsLifetime: ServiceLifetime.Singleton
);
builder.Services.AddDbContextFactory<AuthContext>(
    options => options.UseSqlServer(authConnectionString)
);

var coreConnectionString = builder.Configuration.GetValue<string>("Infrastructure:CoreSqlConnectionString");
builder.Services.AddDbContext<CoreContext>(
    options => options.UseSqlServer(coreConnectionString),
    optionsLifetime: ServiceLifetime.Singleton
);
builder.Services.AddDbContextFactory<CoreContext>(
    options => options.UseSqlServer(coreConnectionString)
);
var blobStorageConnectionString = builder.Configuration.GetValue<string>("Infrastructure:BlobStorageConnectionString");
builder.Services.AddBlobServiceFactory(blobStorageConnectionString);

var serviceBusConnectionString = builder.Configuration.GetValue<string>("Infrastructure:ServiceBusConnectionString");
builder.Services.AddScoped(_ => new ServiceBusClient(serviceBusConnectionString));

// Hosted services
var refreshTokenSleepTime = builder.Configuration.GetValue<int>("Infrastructure:ExpireRefreshTokenIntervalMinutes");
builder.Services.AddScoped<IExpireTokenServiceSettings, ExpireTokenServiceSettings>();
builder.Services.AddScopedTimedHostedService<ExpireSessionsHostedService>(TimeSpan.FromMinutes(refreshTokenSleepTime));

// Middleware
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseHttpsRedirection();
}

app.UseRouting();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();