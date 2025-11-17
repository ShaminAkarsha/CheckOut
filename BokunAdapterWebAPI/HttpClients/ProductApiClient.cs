using BokunAdapterWebAPI.Models;

namespace BokunAdapterWebAPI.HttpClients
{
    public class ProductApiClient
    {
        private readonly HttpClient _http;

        public ProductApiClient(HttpClient http)
        {
            _http = http;
        }

        public async Task<bool> CreateProductAsync(ProductCreateDto dto)
        {
            var response = await _http.PostAsJsonAsync("/api/products", dto);
            return response.IsSuccessStatusCode;
        }
    }

}
