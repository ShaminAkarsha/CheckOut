using Microsoft.EntityFrameworkCore;
using OrderWebAPI;
using OrderWebAPI.http;
using OrderWebAPI.Repositories;
using OrderWebAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

/* Database Configuration */
var dbHost = Environment.GetEnvironmentVariable("DB_HOST");
var dbName = Environment.GetEnvironmentVariable("DB_NAME");
var dbPassword = Environment.GetEnvironmentVariable("DB_SA_PASSWORD");
var connectionString = $"Data Source={dbHost};Initial Catalog={dbName};User ID=sa;Password={dbPassword};TrustServerCertificate=True;";
builder.Services.AddDbContext<OrderDbContext>(opt => opt.UseSqlServer(connectionString));

/* AutoMapper Configuration */
builder.Services.AddAutoMapper(typeof(Program).Assembly);

/* Repository Registration */
builder.Services.AddScoped<IOrderRepository, OrderRepository>();

/* Service Registration */
builder.Services.AddScoped<IOrderService, OrderService>();

/* HttpClient Configuration */
// Add HttpClient for Product Web API
builder.Services.AddHttpClient<ProductClientApi>(client =>
{
    client.BaseAddress = new Uri("http://productwebapi:8080");
    client.Timeout = TimeSpan.FromSeconds(30);
});

// Add HttpClient for Cart Web API
builder.Services.AddHttpClient<CartClientApi>(client =>
{
    client.BaseAddress = new Uri("http://cartwebapi:8080");
    client.Timeout = TimeSpan.FromSeconds(30);
});

/* Swagger Services */
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

/* Logging Configuration */
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Add custom middleware for request logging
app.Use(async (context, next) =>
{
    var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
    logger.LogInformation("Processing request: {Method} {Path}",
        context.Request.Method, context.Request.Path);

    await next();

    logger.LogInformation("Completed request: {Method} {Path} - Status: {StatusCode}",
        context.Request.Method, context.Request.Path, context.Response.StatusCode);
});

app.UseAuthorization();
app.MapControllers();

app.MapGet("/", () => new
{
    Service = "Order WebAPI",
    Status = "Running",
    Version = "1.0.0",
    Timestamp = DateTime.UtcNow
});

app.Run();
