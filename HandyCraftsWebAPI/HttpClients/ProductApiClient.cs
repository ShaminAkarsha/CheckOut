using HandyCraftsAdapterWebAPI.Models;

namespace HandyCraftsAdapterWebAPI.HttpClients
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
            var response = await _http.PostAsJsonAsync("api/product", dto);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> CreateProductsAsync(List<ProductCreateDto> dtos)
        {
            var response = await _http.PostAsJsonAsync("api/product/bulk", dtos);
            Console.WriteLine(response);
            return response.IsSuccessStatusCode;
        }
    }

}
