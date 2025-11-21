using HandyCraftsAdapterWebAPI.HttpClients;
using HandyCraftsAdapterWebAPI.Models;
using System.Text.Json;

namespace HandyCraftsAdapterWebAPI.Services
{
    public class HandyCraftSyncService
    {
        private readonly HttpClient _http;
        private readonly ProductApiClient _productApi;

        public HandyCraftSyncService(HttpClient http, ProductApiClient productApi)
        {
            _http = http;
            _productApi = productApi;
        }

        public async Task<List<HandyCrafts>> LoadFromJsonFile()
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), "Data/products.json");
            string json = await File.ReadAllTextAsync(path);
            return JsonSerializer.Deserialize<List<HandyCrafts>>(json);
        }

        public async Task<List<ProductCreateDto>> SyncAsync()
        {
            var externalProducts = await LoadFromJsonFile();

            var bulkDtos = externalProducts.Select(p => new ProductCreateDto
            {
                ExternalId = p.code,
                Source = "handycrafts",               // Fixed: was "bokun"
                ProductName = p.name,
                ProductCode = p.code,
                ProductPrice = p.price,
                ProductDescription = p.description,
                ProductCategory = "HandyCrafts",
                ProductCoverImage = p.coverImage,
                ProductGalleryImages = p.galleryImages,
                ProductQuantity = p.stockQuantity,
                AdditionalAttributes = new Dictionary<string, object>
                {
                    ["adapter_type"] = "handycrafts",
                    ["material"] = p.material ?? "Unknown",
                    ["size"] = p.size ?? "Unknown",
                    ["color"] = p.color ?? "Unknown",
                    ["weight"] = p.weight ?? "Unknown",
                    ["dimensions"] = p.dimensions ?? "Unknown",
                    ["artisan"] = p.artisan ?? "Unknown",
                    ["origin"] = p.origin ?? "Unknown",
                    ["is_handmade"] = p.isHandmade ?? true,
                    ["crafting_technique"] = p.craftingTechnique ?? "Traditional",
                    ["sync_timestamp"] = DateTime.UtcNow,
                    ["product_category"] = p.category ?? "General"
                }
            }).ToList();

            await _productApi.CreateProductsAsync(bulkDtos);
            return bulkDtos;
        }
    }
}
