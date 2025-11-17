using BokunAdapterWebAPI.HttpClients;
using BokunAdapterWebAPI.Models;
using System.Text.Json;

namespace BokunAdapterWebAPI.Services
{
    public class BokunSyncService
    {
        private readonly HttpClient _http;
        private readonly ProductApiClient _productApi;

        public BokunSyncService(HttpClient http, ProductApiClient productApi)
        {
            _http = http;
            _productApi = productApi;
        }

        public async Task<List<BokunTour>> LoadFromJsonFile()
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), "Data/tours.json");
            string json = await File.ReadAllTextAsync(path);
            return JsonSerializer.Deserialize<List<BokunTour>>(json);
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
                ProductName = p.title,
                ProductCode = p.code,
                ProductPrice = p.price,
                ProductDescription = p.description,
                ProductCategory = "Tours",      // required
                ProductCoverImage = null,
                ProductGalleryImages = new List<string>()
            }).ToList();

            await _productApi.CreateProductsAsync(bulkDtos);
            return bulkDtos;
        }
    }

}
