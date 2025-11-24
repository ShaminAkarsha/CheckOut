using AutoMapper;
using OrderWebAPI.Dtos;
using OrderWebAPI.http;
using OrderWebAPI.Models;
using OrderWebAPI.Repositories;

namespace OrderWebAPI.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly CartClientApi _cartClient;
        private readonly ProductClientApi _productClient;
        private readonly IMapper _mapper;
        private readonly ILogger<OrderService> _logger;

        public OrderService(IOrderRepository orderRepository, CartClientApi cartClient,
                            ProductClientApi productClient, IMapper mapper, ILogger<OrderService> logger)
        {
            _orderRepository = orderRepository;
            _cartClient = cartClient;
            _productClient = productClient;
            _mapper = mapper;
            _logger = logger;
      }

        public async Task<OrderDto?> CreateOrderFromCartAsync(CreateOrderDto createOrderDto)
        {
            try
            {
                _logger.LogInformation("Starting order creation for user: {UserId}", createOrderDto.UserId);

                // Step 1: Fetch cart items from CartAPI
                var cartSummary = await _cartClient.GetCartItemsAsync(createOrderDto.UserId);
                if (cartSummary == null || !cartSummary.Items.Any())
                {
                    _logger.LogWarning("No cart items found for user: {UserId}", createOrderDto.UserId);
                    return null;
                }

                _logger.LogInformation("Found {ItemCount} items in cart for user {UserId}", cartSummary.Items.Count, createOrderDto.UserId);

                // Step 2: Check availability for all items via ProductAPI
                var availabilityChecks = new List<(CartItemDto cartItem, ProductAvailabilityResponseDto availability)>();
        
                foreach (var cartItem in cartSummary.Items)
                {
                    var availabilityRequest = new ProductAvailabilityRequestDto
                    {
                        ProductId = cartItem.ProductId,
                        Quantity = cartItem.Quantity,
                    };

                    var availability = await _productClient.CheckAvailability(availabilityRequest, "ProductService");
                    availabilityChecks.Add((cartItem, availability));
             
                    _logger.LogInformation("Product {ProductId} availability: {IsAvailable}", cartItem.ProductId, availability.IsAvailable);
                }

                // Step 3: Verify all items are available
                var unavailableItems = availabilityChecks.Where(x => !x.availability.IsAvailable).ToList();
                if (unavailableItems.Any())
                {
                    _logger.LogWarning("Some items are not available. Unavailable count: {Count}", unavailableItems.Count);
  
                    // Optional: You could return partial order or detailed error info
                     return null; // or throw exception with details
                }

                _logger.LogInformation("*************************\n********************* Start creating order *********************");


                // Step 4: Create order with items
                var order = new Order
                {
                    CustomerId = createOrderDto.UserId,
                    Status = "Pending",
                    Currency = "USD",
                    Items = availabilityChecks.Select(x => new OrderItem
                    {
                        ProductId = x.availability.ProductId,
                        ProductName = x.availability.ProductName,
                        AdapterId = 1, // Default adapter, you might need logic to determine this
                        Quantity = x.cartItem.Quantity,
                        UnitPrice = x.availability.UnitPrice,
                        TotalPrice = x.availability.UnitPrice * x.cartItem.Quantity,
                        ProviderStatus = "pending"
                    }).ToList()
                };

            // Calculate total amount
            order.TotalAmount = order.Items.Sum(item => item.TotalPrice);

            // Step 5: Save order to database
            var createdOrder = await _orderRepository.CreateOrderAsync(order);
            _logger.LogInformation("Order created with ID: {OrderId}", createdOrder.OrderId);

            // Step 6: Clear cart after successful order creation
            var cartCleared = await _cartClient.ClearCartAsync(createOrderDto.UserId);
            if (!cartCleared)
            {
                _logger.LogWarning("Failed to clear cart after order creation for user: {UserId}", createOrderDto.UserId);
            }

            // Step 7: If all items are available, confirm the order
            createdOrder.Status = "Confirmed";
            await _orderRepository.UpdateOrderAsync(createdOrder);
        
            _logger.LogInformation("Order {OrderId} confirmed successfully", createdOrder.OrderId);

            return _mapper.Map<OrderDto>(createdOrder);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating order for user: {UserId}", createOrderDto.UserId);
                return null;
            }
        }

        public async Task<IEnumerable<OrderSummaryDto>> GetUserOrdersAsync(int userId)
        {
            try
            {
                _logger.LogInformation("Fetching orders for user: {UserId}", userId);

                var orders = await _orderRepository.GetOrdersByUserIdAsync(userId);
                return orders.Select(order => new OrderSummaryDto
                {
                    OrderId = order.OrderId,
                    Status = order.Status,
                    TotalAmount = order.TotalAmount,
                    Currency = order.Currency,
                    CreatedAt = order.CreatedAt,
                    ItemCount = order.Items.Count,
                    ProductNames = order.Items.Select(i => i.ProductName).ToList()
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching orders for user: {UserId}", userId);
                return Enumerable.Empty<OrderSummaryDto>();
            }
        }

   public async Task<OrderDto?> GetOrderDetailsAsync(int orderId, int userId)
   {
       try
       {
            _logger.LogInformation("Fetching order details: OrderId={OrderId}, UserId={UserId}", orderId, userId);
         
            var order = await _orderRepository.GetOrderByIdAsync(orderId, userId);
            if (order == null)
            {
             _logger.LogWarning("Order not found or access denied: OrderId={OrderId}, UserId={UserId}", orderId, userId);
            return null;
            }

            return _mapper.Map<OrderDto>(order);
            }
        catch (Exception ex)
       {
            _logger.LogError(ex, "Error fetching order details: OrderId={OrderId}, UserId={UserId}", orderId, userId);
            return null;
       }
   }

    public async Task<bool> CancelOrderAsync(int orderId, int userId)
    {
        try
        {
            _logger.LogInformation("Cancelling order: OrderId={OrderId}, UserId={UserId}", orderId, userId);
       
           var order = await _orderRepository.GetOrderByIdAsync(orderId, userId);
           if (order == null)
           {
               _logger.LogWarning("Order not found for cancellation: OrderId={OrderId}, UserId={UserId}", orderId, userId);
                return false;
           }

          if (order.Status == "Confirmed" || order.Status == "Pending")
          {
               order.Status = "Cancelled";
               await _orderRepository.UpdateOrderAsync(order);
               _logger.LogInformation("Order cancelled successfully: OrderId={OrderId}", orderId);
               return true;
          }

            _logger.LogWarning("Cannot cancel order in status: {Status}", order.Status);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cancelling order: OrderId={OrderId}, UserId={UserId}", orderId, userId);
            return false;
        }
     }

        public async Task<OrderDto?> UpdateOrderStatusAsync(int orderId, string status)
        {
            try
            {
                _logger.LogInformation("Updating order status: OrderId={OrderId}, Status={Status}", orderId, status);
     
                var order = await _orderRepository.GetOrderByIdAsync(orderId);
                if (order == null)
                {
                    _logger.LogWarning("Order not found for status update: OrderId={OrderId}", orderId);
                    return null;
                }

                order.Status = status;
                await _orderRepository.UpdateOrderAsync(order);
        
                _logger.LogInformation("Order status updated successfully: OrderId={OrderId}, NewStatus={Status}", orderId, status);
                return _mapper.Map<OrderDto>(order);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating order status: OrderId={OrderId}, Status={Status}", orderId, status);
                return null;
            }
        }
    }
}