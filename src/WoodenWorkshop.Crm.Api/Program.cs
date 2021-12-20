using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

using WoodenWorkshop.Core;
using WoodenWorkshop.Core.Contacts.Services;
using WoodenWorkshop.Core.Contacts.Services.Abstractions;
using WoodenWorkshop.Core.Users.Services;
using WoodenWorkshop.Core.Users.Services.Abstractions;
using WoodenWorkshop.Crm.Api.Middleware;
using WoodenWorkshop.Crm.Api.Options;
using WoodenWorkshop.Crm.Api.Services;
using WoodenWorkshop.Crm.Api.Services.Abstractions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<Infrastructure>(builder.Configuration.GetSection("Infrastructure"));
builder.Services.Configure<Security>(builder.Configuration.GetSection("Security"));

builder.Services.AddScoped<IContactsListService, ContactsListService>();
builder.Services.AddScoped<IContactsService, ContactsService>();
builder.Services.AddScoped<IUsersListService, UsersListService>();
builder.Services.AddScoped<IUsersService, UsersService>();
builder.Services.AddScoped<IUserSessionService, UserSessionService>();
builder.Services.AddScoped<ITokenService, JwtService>();

var coreConnectionString = builder.Configuration.GetValue<string>("Infrastructure:CoreSqlConnectionString");
builder.Services.AddDbContext<CoreContext>(
    options => options.UseSqlServer(coreConnectionString),
    optionsLifetime: ServiceLifetime.Singleton
);
builder.Services.AddDbContextFactory<CoreContext>(
    options => options.UseSqlServer(coreConnectionString)
);
var redisConnectionString = builder.Configuration.GetValue<string>("Infrastructure:RedisConnectionString");
var redisConnection = ConnectionMultiplexer.Connect(redisConnectionString);
builder.Services.AddSingleton<IConnectionMultiplexer>(redisConnection);

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

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseAuthorization();
app.MapControllers();

app.Run();