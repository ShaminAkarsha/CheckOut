using BokunAdapterWebAPI.HttpClients;
using BokunAdapterWebAPI.Models;

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

        public async Task SyncAsync()
        {
            var externalProducts = await _http.GetFromJsonAsync<List<BokunTour>>(
                "https://api.bokun.com/products"
            );

            foreach (var p in externalProducts)
            {
                var dto = new ProductCreateDto
                {
                    ProductName = p.title,
                    ProductCode = p.code,
                    ProductPrice = p.price,
                    ProductDescription = p.description
                };

                await _productApi.CreateProductAsync(dto);
            }
        }
    }

}
