using CartWebAPI.Models;

namespace CartWebAPI.Repositories
{
    public interface ICartRepository
    {
        Task<CartItem?> GetCartItemAsync(int userId, int productId);
        Task<CartItem?> GetCartItemByIdAsync(int cartItemId);
        Task<IReadOnlyList<CartItem>> GetUserCartItemsAsync(int userId);
        Task<CartItem> AddCartItemAsync(CartItem cartItem);
        Task<CartItem> UpdateCartItemAsync(CartItem cartItem);
        Task<bool> RemoveCartItemAsync(int cartItemId);
        Task<bool> RemoveCartItemAsync(int userId, int productId);
        Task<bool> ClearUserCartAsync(int userId);
        Task<int> GetCartItemCountAsync(int userId);
        Task<int> GetCartTotalQuantityAsync(int userId);
    }
}