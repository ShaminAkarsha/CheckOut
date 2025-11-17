using BokunAdapterWebAPI.HttpClients;
using BokunAdapterWebAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Add HttpClient for Product web API
builder.Services.AddHttpClient<ProductApiClient>(client =>
{
    client.BaseAddress = new Uri("http://productwebapi:8080");
});

// Add HttpClient for Bokun API

builder.Services.AddHttpClient<BokunSyncService>();


/* Add swager services */
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

app.MapGet("/", () => "This is Bokun Adapter root");

app.Run();
