using CartWebAPI.Models;
using CartWebAPI.Models.DTOs;
using CartWebAPI.Repositories;

namespace CartWebAPI.Services
{
    public interface ICartService
    {
        Task<CartItemDto> AddToCartAsync(AddToCartDto request);
        Task<CartItemDto?> UpdateCartItemAsync(int cartItemId, UpdateCartItemDto request);
        Task<bool> RemoveFromCartAsync(int cartItemId);
        Task<bool> RemoveFromCartAsync(int userId, int productId);
        Task<CartSummaryDto> GetUserCartAsync(int userId);
        Task<bool> ClearCartAsync(int userId);
    }
}