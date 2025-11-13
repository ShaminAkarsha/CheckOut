using CustomerWebAPI;
using JwtAuthenticationManager;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddCustomerJwtAuthentication();

/* Database Context Dependancy Injection */

var dbHost = Environment.GetEnvironmentVariable("DB_HOST");
var dbName = Environment.GetEnvironmentVariable("DB_NAME");
var dbPassword = Environment.GetEnvironmentVariable("DB_SA_PASSWORD");
//var dbUser = "sqluser";
//var connectionString = "Server=CL-AKARSHAS\\SQLEXPRESS;Database=dms_customer;Trusted_Connection=True;TrustServerCertificate=True;";
var connectionString = $"Data Source={dbHost};Initial Catalog={dbName};User ID=sa;Password={dbPassword};TrustServerCertificate=True;";
builder.Services.AddDbContext<CustomerDbContext>(opt => opt.UseSqlServer(connectionString)); // By default, AddDbContext registers your context as a scoped service.

/* ===================================== */

/* Swagger Services */
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Configure the HTTP request pipeline.

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapGet("/", () => "This is root");

app.Run();
