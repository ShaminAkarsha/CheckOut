using CartWebAPI.Models;
using CartWebAPI.Models.DTOs;
using CartWebAPI.Repositories;

namespace CartWebAPI.Services
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly ICartValidationService _validationService;
        private readonly ILogger<CartService> _logger;

        public CartService(
            ICartRepository cartRepository,
            ICartValidationService validationService,
            ILogger<CartService> logger)
        {
            _cartRepository = cartRepository;
            _validationService = validationService;
            _logger = logger;
        }

        public async Task<CartItemDto> AddToCartAsync(AddToCartDto request)
        {
            // Validate request
            var validationResult = _validationService.ValidateAddToCartRequest(request);
            if (!validationResult.IsValid)
            {
                var errorMessage = string.Join("; ", validationResult.Errors);
                _logger.LogWarning("Validation failed for AddToCart: {Errors}", errorMessage);
                throw new ArgumentException($"Validation failed: {errorMessage}");
            }

            // Additional async validations
            if (!await _validationService.ValidateUserExistsAsync(request.UserId))
            {
                _logger.LogWarning("User not found: {UserId}", request.UserId);
                throw new ArgumentException($"User with ID {request.UserId} not found");
            }

            if (!await _validationService.ValidateProductExistsAsync(request.ProductId))
            {
                _logger.LogWarning("Product not found: {ProductId}", request.ProductId);
                throw new ArgumentException($"Product with ID {request.ProductId} not found");
            }

            // Check if item already exists in cart
            var existingItem = await _cartRepository.GetCartItemAsync(request.UserId, request.ProductId);

            if (existingItem != null)
            {
                // Validate total quantity doesn't exceed limits
                var newQuantity = existingItem.Quantity + request.Quantity;
                if (newQuantity > 100)
                {
                    _logger.LogWarning("Quantity limit exceeded for User {UserId}, Product {ProductId}. Attempted: {NewQuantity}",
                        request.UserId, request.ProductId, newQuantity);
                    throw new ArgumentException("Total quantity cannot exceed 100 items");
                }

                // Update quantity if item exists
                existingItem.Quantity = newQuantity;
                var updatedItem = await _cartRepository.UpdateCartItemAsync(existingItem);
                _logger.LogInformation("Updated cart item quantity for User {UserId}, Product {ProductId}. New quantity: {Quantity}",
                    request.UserId, request.ProductId, updatedItem.Quantity);
                return MapToDto(updatedItem);
            }
            else
            {
                // Add new item to cart
                var newCartItem = new CartItem
                {
                    UserId = request.UserId,
                    ProductId = request.ProductId,
                    Quantity = request.Quantity,
                    CreatedDate = DateTime.UtcNow,
                    UpdatedDate = DateTime.UtcNow
                };

                var addedItem = await _cartRepository.AddCartItemAsync(newCartItem);
                _logger.LogInformation("Added new item to cart for User {UserId}, Product {ProductId}, Quantity: {Quantity}",
                    request.UserId, request.ProductId, request.Quantity);
                return MapToDto(addedItem);
            }
        }

        public async Task<CartItemDto?> UpdateCartItemAsync(int cartItemId, UpdateCartItemDto request)
        {
            // Validate request
            var validationResult = _validationService.ValidateUpdateCartRequest(request);
            if (!validationResult.IsValid)
            {
                var errorMessage = string.Join("; ", validationResult.Errors);
                _logger.LogWarning("Validation failed for UpdateCartItem: {Errors}", errorMessage);
                throw new ArgumentException($"Validation failed: {errorMessage}");
            }

            var cartItem = await _cartRepository.GetCartItemByIdAsync(cartItemId);
            if (cartItem == null)
            {
                _logger.LogWarning("Cart item not found with ID {CartItemId}", cartItemId);
                return null;
            }

            cartItem.Quantity = request.Quantity;
            var updatedItem = await _cartRepository.UpdateCartItemAsync(cartItem);
            _logger.LogInformation("Updated cart item {CartItemId} quantity to {Quantity}", cartItemId, request.Quantity);
            return MapToDto(updatedItem);
        }

        public async Task<bool> RemoveFromCartAsync(int cartItemId)
        {
            var success = await _cartRepository.RemoveCartItemAsync(cartItemId);
            if (success)
            {
                _logger.LogInformation("Removed cart item with ID {CartItemId}", cartItemId);
            }
            else
            {
                _logger.LogWarning("Failed to remove cart item with ID {CartItemId} - item not found", cartItemId);
            }
            return success;
        }

        public async Task<bool> RemoveFromCartAsync(int userId, int productId)
        {
            var success = await _cartRepository.RemoveCartItemAsync(userId, productId);
            if (success)
            {
                _logger.LogInformation("Removed cart item for User {UserId}, Product {ProductId}", userId, productId);
            }
            else
            {
                _logger.LogWarning("Failed to remove cart item for User {UserId}, Product {ProductId} - item not found", userId, productId);
            }
            return success;
        }

        public async Task<CartSummaryDto> GetUserCartAsync(int userId)
        {
            if (userId <= 0)
            {
                throw new ArgumentException("UserId must be greater than 0");
            }

            var cartItems = await _cartRepository.GetUserCartItemsAsync(userId);
            var cartItemDtos = cartItems.Select(MapToDto).ToList();

            return new CartSummaryDto
            {
                UserId = userId,
                Items = cartItemDtos,
                TotalItems = cartItemDtos.Count,
                TotalQuantity = cartItemDtos.Sum(x => x.Quantity)
            };
        }

        public async Task<bool> ClearCartAsync(int userId)
        {
            if (userId <= 0)
            {
                throw new ArgumentException("UserId must be greater than 0");
            }

            var success = await _cartRepository.ClearUserCartAsync(userId);
            if (success)
            {
                _logger.LogInformation("Cleared cart for User {UserId}", userId);
            }
            else
            {
                _logger.LogInformation("No items found to clear for User {UserId}", userId);
            }
            return success;
        }

        private static CartItemDto MapToDto(CartItem cart)
        {
            return new CartItemDto
            {
                CartItemId = cart.CartItemId,
                UserId = cart.UserId,
                ProductId = cart.ProductId,
                Quantity = cart.Quantity,
                CreatedDate = cart.CreatedDate,
                UpdatedDate = cart.UpdatedDate
            };
        }
    }
}