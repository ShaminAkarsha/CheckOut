using HandyCraftsAdapterWebAPI.HttpClients;
using HandyCraftsAdapterWebAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

/* Add Swagger */
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add HttpClient for Product web API
builder.Services.AddHttpClient<ProductApiClient>(client =>
{
    client.BaseAddress = new Uri("http://productwebapi:8080");
});

// Add HttpClient for Bokun API

builder.Services.AddHttpClient<HandyCraftSyncService>();



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

app.MapGet("/", () => "Welcome to Handy Crafts Web API");

app.Run();
