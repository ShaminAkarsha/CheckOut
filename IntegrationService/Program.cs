using IntegrationService;
using IntegrationService.Data;
using IntegrationService.Repositories;
using IntegrationService.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

/* Add swager services */
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

/* Add HttpClient services */
builder.Services.AddHttpClient();

/* Database Context Dependancy Injection */
var dbHost = Environment.GetEnvironmentVariable("DB_HOST");
var dbName = Environment.GetEnvironmentVariable("DB_NAME");
var dbPassword = Environment.GetEnvironmentVariable("DB_SA_PASSWORD");
var connectionString = $"Data Source={dbHost};Initial Catalog={dbName};User ID=sa;Password={dbPassword};TrustServerCertificate=True;";
builder.Services.AddDbContext<IntegrationDbContext>(opt => opt.UseSqlServer(connectionString));

builder.Services.AddScoped<IAdapterRepository, AdapterRepository>();
builder.Services.AddScoped<IAdapterClient, AdapterClient>();
builder.Services.AddScoped<IIntegrationService, IntegrationService.Services.IntegrationService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<IntegrationDbContext>();
    await DbSeeder.SeedAsync(db);
}

app.MapGet("/", () => "Integration service up!");

app.Run();
