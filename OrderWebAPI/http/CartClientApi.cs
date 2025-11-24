using OrderWebAPI.Dtos;
using System.Text.Json;

namespace OrderWebAPI.http
{
    public class CartClientApi
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<CartClientApi> _logger;

        public CartClientApi(HttpClient httpClient, ILogger<CartClientApi> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        /// <summary>
        /// Get all cart items for a specific user
        /// </summary>
        public async Task<CartSummaryDto?> GetCartItemsAsync(int userId)
        {
            try
            {
                _logger.LogInformation("Fetching cart items for user: {UserId}", userId);

                var response = await _httpClient.GetAsync($"api/cart/user/{userId}");

                if (response.IsSuccessStatusCode)
                {
                    var cartSummary = await response.Content.ReadFromJsonAsync<CartSummaryDto>();
                    _logger.LogInformation("Retrieved {ItemCount} cart items for user {UserId}",
                        cartSummary?.Items?.Count ?? 0, userId);
                    return cartSummary;
                }

                _logger.LogWarning("Failed to fetch cart items. Status: {StatusCode}", response.StatusCode);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching cart items for user: {UserId}", userId);
                return null;
            }
        }

        /// <summary>
        /// Clear all items from user's cart
        /// </summary>
        public async Task<bool> ClearCartAsync(int userId)
        {
            try
            {
                _logger.LogInformation("Clearing cart for user: {UserId}", userId);

                var response = await _httpClient.DeleteAsync($"api/cart/{userId}/clear");

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Successfully cleared cart for user: {UserId}", userId);
                    return true;
                }

                _logger.LogWarning("Failed to clear cart. Status: {StatusCode}", response.StatusCode);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error clearing cart for user: {UserId}", userId);
                return false;
            }
        }

        /// <summary>
        /// Remove specific item from cart
        /// </summary>
        public async Task<bool> RemoveCartItemAsync(int userId, int cartItemId)
        {
            try
            {
                _logger.LogInformation("Removing cart item {CartItemId} for user: {UserId}", cartItemId, userId);

                var response = await _httpClient.DeleteAsync($"api/cart/{userId}/item/{cartItemId}");

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Successfully removed cart item {CartItemId}", cartItemId);
                    return true;
                }

                _logger.LogWarning("Failed to remove cart item. Status: {StatusCode}", response.StatusCode);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing cart item {CartItemId} for user: {UserId}", cartItemId, userId);
                return false;
            }
        }
    }
}
