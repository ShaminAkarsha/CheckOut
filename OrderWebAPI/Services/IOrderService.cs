using OrderWebAPI.Dtos;
using OrderWebAPI.Models;

namespace OrderWebAPI.Services
{
    public interface IOrderService
 {
    Task<OrderDto?> CreateOrderFromCartAsync(CreateOrderDto createOrderDto);
    Task<IEnumerable<OrderSummaryDto>> GetUserOrdersAsync(int userId);
    Task<OrderDto?> GetOrderDetailsAsync(int orderId, int userId);
    Task<bool> CancelOrderAsync(int orderId, int userId);
    Task<OrderDto?> UpdateOrderStatusAsync(int orderId, string status);
  }
}