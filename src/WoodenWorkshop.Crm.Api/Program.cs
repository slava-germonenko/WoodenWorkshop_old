using System.Text;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

using WoodenWorkshop.Auth;
using WoodenWorkshop.Auth.Jobs;
using WoodenWorkshop.Auth.Jobs.Settings;
using WoodenWorkshop.Auth.Services;
using WoodenWorkshop.Auth.Services.Abstractions;
using WoodenWorkshop.Core;
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

var builder = WebApplication.CreateBuilder(args);

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

builder.Services.AddScoped<IContactsListService, ContactsListService>();
builder.Services.AddScoped<IContactsService, ContactsService>();
builder.Services.AddScoped<IExpireRefreshTokensSettings, ExpireRefreshTokensSettings>();
builder.Services.AddScoped<IRolePermissionsService, RolePermissionsService>();
builder.Services.AddScoped<IRolesListService, RolesListService>();
builder.Services.AddScoped<IRolesService, RolesService>();
builder.Services.AddScoped<ISessionsExpirationService, SessionsExpirationService>();
builder.Services.AddScoped<IUserRolesService, UserRolesService>();
builder.Services.AddScoped<IUsersListService, UsersListService>();
builder.Services.AddScoped<IUsersService, UsersService>();
builder.Services.AddScoped<IUserSessionService, UserSessionService>();
builder.Services.AddScoped<ITokenService, JwtService>();

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

builder.Services.AddHostedService<ExpireRefreshTokenBackgroundService>();

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