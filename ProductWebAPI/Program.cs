using Microsoft.EntityFrameworkCore;
using ProductWebAPI;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

/* Swagger Services */
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

/* Database Context Dependancy Injection */
var dbHost = Environment.GetEnvironmentVariable("DB_HOST");
var dbName = Environment.GetEnvironmentVariable("DB_NAME");
var dbPassword = Environment.GetEnvironmentVariable("DB_SA_PASSWORD");
var connectionString = $"Data Source={dbHost};Initial Catalog={dbName};User ID=sa;Password={dbPassword};TrustServerCertificate=True;";
builder.Services.AddDbContext<ProductDbContext>(opt => opt.UseSqlServer(connectionString));

/* ====================================== */

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.MapGet("/", () => "This is Root");

app.Run();
