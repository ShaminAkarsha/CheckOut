using OrderWebAPI.Dtos;

namespace OrderWebAPI.http
{
    public class ProductClientApi
    {
        private readonly HttpClient _http;

        public ProductClientApi(HttpClient http)
        {
            _http = http; 
        }

        public async Task<ProductAvailabilityResponseDto> CheckAvailability(ProductAvailabilityRequestDto dto, string source)
        {
            try
            {
                var response = await _http.PostAsJsonAsync($"api/product/availability", dto);

                if (response.IsSuccessStatusCode)
                {
                    var availabilityResponse = await response.Content.ReadFromJsonAsync<ProductAvailabilityResponseDto>();
                    return availabilityResponse ?? new ProductAvailabilityResponseDto
                    {
                        IsAvailable = false,
                        Message = "Failed to deserialize response",
                        ProductCode = string.Empty,
                        ProductName = string.Empty,
                        Source = source
                    };
                }
                else
                {
                    return new ProductAvailabilityResponseDto
                    {
                        IsAvailable = false,
                        Message = $"API call failed with status: {response.StatusCode}",
                        ProductCode = string.Empty,
                        ProductName = string.Empty,
                        Source = source
                    };
                }
            }
            catch (Exception ex)
            {
                return new ProductAvailabilityResponseDto
                {
                    IsAvailable = false,
                    Message = $"Error calling availability API: {ex.Message}",
                    ProductCode = string.Empty,
                    ProductName = string.Empty,
                    Source = source
                };
            }
        }
    }
}
