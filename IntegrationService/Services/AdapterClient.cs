using IntegrationService.Models;
using System.Text.Json;
using System.Text;

namespace IntegrationService.Services
{
    public class AdapterClient : IAdapterClient
    {
        private readonly HttpClient _http;

        public AdapterClient(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<ProductCreateDto>> SyncProductsAsync(Adapter adapter)
        {
            var url = $"{adapter.BaseUrl}/adapter/sync/products";
            
            // Add authentication headers if ApiKey exists
            if (!string.IsNullOrEmpty(adapter.ApiKey))
            {
                _http.DefaultRequestHeaders.Authorization = 
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", adapter.ApiKey);
            }

            // Add custom headers if they exist
            if (!string.IsNullOrEmpty(adapter.CustomHeadersJson))
            {
                try
                {
                    var customHeaders = JsonSerializer.Deserialize<Dictionary<string, string>>(adapter.CustomHeadersJson);
                    if (customHeaders != null)
                    {
                        foreach (var header in customHeaders)
                        {
                            _http.DefaultRequestHeaders.Add(header.Key, header.Value);
                        }
                    }
                }
                catch (JsonException)
                {
                    // Log error or handle invalid JSON
                }
            }

            // Create empty JSON content for POST request
            var content = new StringContent("{}", Encoding.UTF8, "application/json");
            var response = await _http.PostAsync(url, content);
            
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<List<ProductCreateDto>>();
                return result ?? new List<ProductCreateDto>();
            }

            return new List<ProductCreateDto>();
        }

        public async Task<bool> CheckAvailabilityAsync(Adapter adapter, string productId)
        {
            var url = $"{adapter.BaseUrl}/adapter/availability/{productId}";
            
            try
            {
                var result = await _http.GetFromJsonAsync<bool>(url);
                return result;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> ProcessPaymentsAsync(Adapter adapter, string productId)
        {
            var url = $"{adapter.BaseUrl}/adapter/payments/{productId}";
            var content = new StringContent("{}", Encoding.UTF8, "application/json");
            
            try
            {
                var result = await _http.PostAsync(url, content);
                return result.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
    }
}
