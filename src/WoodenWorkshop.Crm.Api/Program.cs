using Microsoft.EntityFrameworkCore;

using WoodenWorkshop.Core;
using WoodenWorkshop.Core.Contacts.Services;
using WoodenWorkshop.Core.Contacts.Services.Abstractions;
using WoodenWorkshop.Crm.Api.Middleware;
using WoodenWorkshop.Crm.Api.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<Infrastructure>(builder.Configuration.GetSection("Infrastructure"));

builder.Services.AddScoped<IContactsService, ContactsService>();
builder.Services.AddScoped<IContactsListService, ContactsListService>();

var coreConnectionString = builder.Configuration
    .GetSection("Infrastructure")
    .GetValue<string>("CoreSqlConnectionString");
builder.Services.AddDbContext<CoreContext>(
    options => options.UseSqlServer(coreConnectionString),
    optionsLifetime: ServiceLifetime.Singleton
);
builder.Services.AddDbContextFactory<CoreContext>(
    options => options.UseSqlServer(coreConnectionString)
);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseAuthorization();
app.MapControllers();

app.Run();