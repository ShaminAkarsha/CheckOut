using Microsoft.EntityFrameworkCore;
using OrderWebAPI;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var dbHost = Environment.GetEnvironmentVariable("DB_HOST");
var dbName = Environment.GetEnvironmentVariable("DB_NAME");
var dbPassword = Environment.GetEnvironmentVariable("DB_SA_PASSWORD");
//var dbUser = "sqluser";
//var connectionString = "Server=CL-AKARSHAS\\SQLEXPRESS;Database=dms_customer;Trusted_Connection=True;TrustServerCertificate=True;";
var connectionString = $"Data Source={dbHost};Initial Catalog={dbName};User ID=sa;Password={dbPassword};TrustServerCertificate=True;";
builder.Services.AddDbContext<OrderDbContext>(opt => opt.UseSqlServer(connectionString)); // By default, AddDbContext registers your context as a scoped service.


/* Swagger Services */
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

app.MapGet("/", () => "This is Order API root");

app.Run();
