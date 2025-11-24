using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrderWebAPI.Dtos;
using OrderWebAPI.Services;

namespace OrderWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly ILogger<OrderController> _logger;

        public OrderController(IOrderService orderService, ILogger<OrderController> logger)
      {
            _orderService = orderService;
            _logger = logger;
        }

    /// <summary>
    /// Get all orders from order db by user id
    /// </summary>
    /// <param name="userId">User ID to fetch orders for</param>
    /// <returns>List of order summaries for the user</returns>
    [HttpGet("{userId:int}")]
    public async Task<ActionResult<IEnumerable<OrderSummaryDto>>> GetUserOrders(int userId)
    {
        try
        {
            _logger.LogInformation("Fetching orders for user: {UserId}", userId);

            if (userId <= 0)
            {
               return BadRequest("Invalid user ID");
            }   

            var orders = await _orderService.GetUserOrdersAsync(userId);
 
            _logger.LogInformation("Retrieved {OrderCount} orders for user: {UserId}", orders.Count(), userId);

            return Ok(orders);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching orders for user: {UserId}", userId);
            return StatusCode(500, "An error occurred while fetching orders");
        }
    }

    /// <summary>
    /// Fetch order by id from order db and user id
    /// </summary>
    /// <param name="userId">User ID who owns the order</param>
    /// <param name="orderId">Order ID to fetch</param>
    /// <returns>Detailed order information</returns>
    [HttpGet("{userId:int}/{orderId:int}")]
    public async Task<ActionResult<OrderDto>> GetOrderDetails(int userId, int orderId)
    {
        try
        {
            _logger.LogInformation("Fetching order details: OrderId={OrderId}, UserId={UserId}", orderId, userId);

            if (userId <= 0 || orderId <= 0)
            {
                return BadRequest("Invalid user ID or order ID");
            }

            var order = await _orderService.GetOrderDetailsAsync(orderId, userId);
                
            if (order == null)
            {
                _logger.LogWarning("Order not found or access denied: OrderId={OrderId}, UserId={UserId}",  orderId, userId);
                return NotFound("Order not found or you don't have permission to access it");
             }

            _logger.LogInformation("Successfully retrieved order: OrderId={OrderId}", orderId);
            return Ok(order);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching order details: OrderId={OrderId}, UserId={UserId}", orderId, userId);
            return StatusCode(500, "An error occurred while fetching order details");
        }
    }

    /// <summary>
    /// Create new order from user's cart
    /// Process: 
    /// 1. Fetch cart items from cartApi 
    /// 2. Check all items availability from productApi
    /// 3. If all items are available, create and confirm the order
    /// 4. Save it in the order db
    /// 5. Clear the cart using cartApi
    /// 6. Return order details with ordered items 
    /// </summary>
    /// <param name="createOrderDto">Order creation request</param>
    /// <returns>Created order details</returns>
    [HttpPost]
    public async Task<ActionResult<OrderDto>> CreateOrder([FromBody] CreateOrderDto createOrderDto)
    {
        try
        {
            _logger.LogInformation("Creating order for user: {UserId}", createOrderDto.UserId);

            // Validate input
            if (createOrderDto.UserId <= 0)
            {
                return BadRequest("Invalid user ID");
            }

            // Execute the complete order creation workflow
            var createdOrder = await _orderService.CreateOrderFromCartAsync(createOrderDto);
     
            if (createdOrder == null)
            {
                _logger.LogWarning("Failed to create order for user: {UserId}. " + "Possible reasons: empty cart, unavailable items, or system error", createOrderDto.UserId);
       
                return BadRequest(new
                {
                    message = "Unable to create order",
                    possibleReasons = new[]
                    {
                      "Cart is empty",
                      "One or more items are not available",
                      "Insufficient stock for requested quantities",
                      "System error occurred"
                        }
                });
            }

        
                _logger.LogInformation("Successfully created order: OrderId={OrderId}, UserId={UserId}, " +
          "TotalAmount={TotalAmount}", 
          createdOrder.OrderId, createOrderDto.UserId, createdOrder.TotalAmount);

      return CreatedAtAction(
   nameof(GetOrderDetails), 
  new { userId = createOrderDto.UserId, orderId = createdOrder.OrderId }, 
            createdOrder);
       }
       catch (Exception ex)
     {
         _logger.LogError(ex, "Error creating order for user: {UserId}", createOrderDto.UserId);
    return StatusCode(500, "An error occurred while creating the order");
            }
        }

  /// <summary>
     /// Cancel an existing order
        /// </summary>
        /// <param name="userId">User ID who owns the order</param>
        /// <param name="orderId">Order ID to cancel</param>
        /// <returns>Cancellation result</returns>
        [HttpPut("{userId:int}/{orderId:int}/cancel")]
        public async Task<ActionResult> CancelOrder(int userId, int orderId)
        {
            try
   {
          _logger.LogInformation("Cancelling order: OrderId={OrderId}, UserId={UserId}", 
        orderId, userId);

             if (userId <= 0 || orderId <= 0)
          {
 return BadRequest("Invalid user ID or order ID");
      }

  var cancelled = await _orderService.CancelOrderAsync(orderId, userId);
         
     if (!cancelled)
           {
       _logger.LogWarning("Failed to cancel order: OrderId={OrderId}, UserId={UserId}. " +
         "Order may not exist, belong to another user, or be in non-cancellable status", 
        orderId, userId);
   
       return BadRequest(new
              {
     message = "Unable to cancel order",
        possibleReasons = new[]
           {
         "Order not found",
     "Order doesn't belong to the specified user",
                   "Order is already completed, shipped, or cancelled",
  "Order cannot be cancelled in its current status"
            }
     });
              }

    _logger.LogInformation("Successfully cancelled order: OrderId={OrderId}", orderId);
              return Ok(new { message = "Order cancelled successfully", orderId = orderId });
 }
            catch (Exception ex)
         {
           _logger.LogError(ex, "Error cancelling order: OrderId={OrderId}, UserId={UserId}", 
            orderId, userId);
     return StatusCode(500, "An error occurred while cancelling the order");
            }
        }

        /// <summary>
        /// Update order status (Admin functionality)
     /// </summary>
        /// <param name="orderId">Order ID to update</param>
        /// <param name="status">New status</param>
        /// <returns>Updated order details</returns>
        [HttpPut("{orderId:int}/status")]
        public async Task<ActionResult<OrderDto>> UpdateOrderStatus(int orderId, [FromQuery] string status)
        {
      try
  {
       _logger.LogInformation("Updating order status: OrderId={OrderId}, NewStatus={Status}", 
          orderId, status);

                if (orderId <= 0 || string.IsNullOrWhiteSpace(status))
    {
           return BadRequest("Invalid order ID or status");
       }

       var validStatuses = new[] { "Pending", "Confirmed", "Processing", "Shipped", "Delivered", "Cancelled", "Failed" };
             if (!validStatuses.Contains(status))
    {
      return BadRequest($"Invalid status. Valid statuses are: {string.Join(", ", validStatuses)}");
          }

    var updatedOrder = await _orderService.UpdateOrderStatusAsync(orderId, status);
   
    if (updatedOrder == null)
    {
        _logger.LogWarning("Failed to update order status: OrderId={OrderId} not found", orderId);
           return NotFound("Order not found");
     }

       _logger.LogInformation("Successfully updated order status: OrderId={OrderId}, NewStatus={Status}", 
         orderId, status);

       return Ok(updatedOrder);
    }
  catch (Exception ex)
      {
     _logger.LogError(ex, "Error updating order status: OrderId={OrderId}, Status={Status}", 
     orderId, status);
    return StatusCode(500, "An error occurred while updating order status");
            }
   }

        /// <summary>
        /// Health check endpoint for order service
     /// </summary>
        /// <returns>Service status</returns>
        [HttpGet("health")]
        public ActionResult GetHealth()
        {
            return Ok(new 
    { 
        service = "OrderWebAPI", 
        status = "healthy", 
          timestamp = DateTime.UtcNow,
     version = "1.0.0"
  });
        }
    }
}
