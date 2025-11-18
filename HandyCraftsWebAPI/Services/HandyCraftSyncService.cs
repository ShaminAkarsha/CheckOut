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

            //var externalProducts = await _http.GetFromJsonAsync<List<BokunTour>>(
            //    "https://api.bokun.com/products"
            //);

            var bulkDtos = externalProducts.Select(p => new ProductCreateDto
            {
                ExternalId = p.code,            // required
                Source = "bokun",               // required
                ProductName = p.name,
                ProductCode = p.code,
                ProductPrice = p.price,
                ProductDescription = p.description,
                ProductCategory = "HandyCrafts",      // required
                ProductCoverImage = p.coverImage,
                ProductGalleryImages = p.galleryImages
            }).ToList();

            await _productApi.CreateProductsAsync(bulkDtos);
            return bulkDtos;
        }
    }
}
