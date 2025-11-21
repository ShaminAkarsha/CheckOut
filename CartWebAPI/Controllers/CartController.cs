using CartWebAPI.Models.DTOs;
using CartWebAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace CartWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;
        private readonly ILogger<CartController> _logger;

        public CartController(ICartService cartService, ILogger<CartController> logger)
        {
     _cartService = cartService;
     _logger = logger;
        }

        /// <summary>
     /// Add item to cart. If item already exists, quantity will be increased.
  /// </summary>
   /// <param name="request">Cart item details</param>
  /// <returns>Created cart item</returns>
   [HttpPost]
        [ProducesResponseType(typeof(CartItemDto), StatusCodes.Status201Created)]
[ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
   public async Task<ActionResult<CartItemDto>> AddToCart([FromBody] AddToCartDto request)
        {
      if (!ModelState.IsValid)
     {
     return BadRequest(ModelState);
        }

   try
    {
     var result = await _cartService.AddToCartAsync(request);
       return CreatedAtAction(nameof(GetUserCart), new { userId = result.UserId }, result);
     }
     catch (ArgumentException ex)
       {
       _logger.LogWarning(ex, "Invalid request for adding item to cart: {Message}", ex.Message);
      return BadRequest(new { error = ex.Message });
     }
     catch (Exception ex)
   {
         _logger.LogError(ex, "Error adding item to cart for User {UserId}, Product {ProductId}", 
        request.UserId, request.ProductId);
     return StatusCode(500, new { error = "An error occurred while adding item to cart" });
     }
 }

        /// <summary>
  /// Get all cart items for a specific user
     /// </summary>
  /// <param name="userId">User ID</param>
        /// <returns>User's cart summary</returns>
     [HttpGet("user/{userId:int}")]
   [ProducesResponseType(typeof(CartSummaryDto), StatusCodes.Status200OK)]
   [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<CartSummaryDto>> GetUserCart(int userId)
        {
     if (userId <= 0)
     {
     return BadRequest(new { error = "UserId must be greater than 0" });
        }

      try
     {
        var result = await _cartService.GetUserCartAsync(userId);
      return Ok(result);
  }
      catch (ArgumentException ex)
      {
   _logger.LogWarning(ex, "Invalid request for getting user cart: {Message}", ex.Message);
    return BadRequest(new { error = ex.Message });
       }
  catch (Exception ex)
   {
     _logger.LogError(ex, "Error retrieving cart for User {UserId}", userId);
      return StatusCode(500, new { error = "An error occurred while retrieving cart" });
   }
   }

    /// <summary>
        /// Update quantity of a specific cart item
   /// </summary>
        /// <param name="cartItemId">Cart item ID</param>
        /// <param name="request">Updated quantity</param>
   /// <returns>Updated cart item</returns>
     [HttpPut("{cartItemId:int}")]
      [ProducesResponseType(typeof(CartItemDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
   [ProducesResponseType(StatusCodes.Status404NotFound)]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<CartItemDto>> UpdateCartItem(int cartItemId, [FromBody] UpdateCartItemDto request)
        {
     if (!ModelState.IsValid)
   {
      return BadRequest(ModelState);
     }

    if (cartItemId <= 0)
  {
    return BadRequest(new { error = "CartItemId must be greater than 0" });
        }

    try
       {
        var result = await _cartService.UpdateCartItemAsync(cartItemId, request);
  if (result == null)
      {
      return NotFound(new { error = $"Cart item with ID {cartItemId} not found" });
        }
      return Ok(result);
       }
       catch (ArgumentException ex)
        {
     _logger.LogWarning(ex, "Invalid request for updating cart item: {Message}", ex.Message);
    return BadRequest(new { error = ex.Message });
     }
     catch (Exception ex)
     {
     _logger.LogError(ex, "Error updating cart item {CartItemId}", cartItemId);
        return StatusCode(500, new { error = "An error occurred while updating cart item" });
      }
    }

     /// <summary>
     /// Remove a specific cart item by cart item ID
      /// </summary>
        /// <param name="cartItemId">Cart item ID</param>
        /// <returns>No content if successful</returns>
  [HttpDelete("{cartItemId:int}")]
     [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RemoveFromCart(int cartItemId)
   {
    if (cartItemId <= 0)
        {
    return BadRequest(new { error = "CartItemId must be greater than 0" });
     }

     try
        {
      var success = await _cartService.RemoveFromCartAsync(cartItemId);
        if (!success)
   {
      return NotFound(new { error = $"Cart item with ID {cartItemId} not found" });
      }
            return NoContent();
     }
       catch (Exception ex)
      {
         _logger.LogError(ex, "Error removing cart item {CartItemId}", cartItemId);
      return StatusCode(500, new { error = "An error occurred while removing cart item" });
    }
        }

        /// <summary>
     /// Remove a specific product from user's cart
     /// </summary>
   /// <param name="userId">User ID</param>
        /// <param name="productId">Product ID</param>
   /// <returns>No content if successful</returns>
   [HttpDelete("user/{userId:int}/product/{productId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
     [ProducesResponseType(StatusCodes.Status404NotFound)]
  [ProducesResponseType(StatusCodes.Status500InternalServerError)]
     public async Task<IActionResult> RemoveFromCart(int userId, int productId)
     {
    if (userId <= 0)
     {
    return BadRequest(new { error = "UserId must be greater than 0" });
        }

     if (productId <= 0)
        {
     return BadRequest(new { error = "ProductId must be greater than 0" });
    }

      try
{
    var success = await _cartService.RemoveFromCartAsync(userId, productId);
       if (!success)
     {
       return NotFound(new { error = $"Cart item for User {userId} and Product {productId} not found" });
 }
     return NoContent();
       }
     catch (Exception ex)
      {
     _logger.LogError(ex, "Error removing cart item for User {UserId}, Product {ProductId}", userId, productId);
     return StatusCode(500, new { error = "An error occurred while removing cart item" });
        }
     }

        /// <summary>
   /// Clear all items from user's cart
        /// </summary>
   /// <param name="userId">User ID</param>
     /// <returns>No content if successful</returns>
        [HttpDelete("user/{userId:int}/clear")]
      [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
  [ProducesResponseType(StatusCodes.Status500InternalServerError)]
     public async Task<IActionResult> ClearCart(int userId)
        {
      if (userId <= 0)
  {
       return BadRequest(new { error = "UserId must be greater than 0" });
        }

        try
   {
  var success = await _cartService.ClearCartAsync(userId);
        if (!success)
        {
     return NotFound(new { error = $"No cart items found for User {userId}" });
        }
     return NoContent();
      }
       catch (ArgumentException ex)
  {
     _logger.LogWarning(ex, "Invalid request for clearing cart: {Message}", ex.Message);
       return BadRequest(new { error = ex.Message });
      }
     catch (Exception ex)
      {
  _logger.LogError(ex, "Error clearing cart for User {UserId}", userId);
 return StatusCode(500, new { error = "An error occurred while clearing cart" });
 }
        }

        /// <summary>
     /// Get cart summary (total items and quantity) for a user
    /// </summary>
      /// <param name="userId">User ID</param>
      /// <returns>Cart summary</returns>
[HttpGet("user/{userId:int}/summary")]
 [ProducesResponseType(StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
     public async Task<ActionResult<object>> GetCartSummary(int userId)
      {
     if (userId <= 0)
      {
        return BadRequest(new { error = "UserId must be greater than 0" });
        }

     try
      {
    var cart = await _cartService.GetUserCartAsync(userId);
    return Ok(new
     {
        UserId = userId,
     TotalItems = cart.TotalItems,
       TotalQuantity = cart.TotalQuantity,
        LastUpdated = DateTime.UtcNow
        });
        }
     catch (ArgumentException ex)
   {
     _logger.LogWarning(ex, "Invalid request for getting cart summary: {Message}", ex.Message);
      return BadRequest(new { error = ex.Message });
     }
      catch (Exception ex)
      {
      _logger.LogError(ex, "Error retrieving cart summary for User {UserId}", userId);
        return StatusCode(500, new { error = "An error occurred while retrieving cart summary" });
     }
    }
    }
}