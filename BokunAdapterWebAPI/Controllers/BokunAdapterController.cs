using BokunAdapterWebAPI.Models;
using BokunAdapterWebAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace BokunAdapterWebAPI.Controllers
{
    [Route("api/adapter")]
    [ApiController]
    public class BokunAdapterController : ControllerBase
    {
        private readonly BokunSyncService _service;
        private readonly ILogger<BokunAdapterController> _logger;

        public BokunAdapterController(BokunSyncService service, ILogger<BokunAdapterController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpPost("sync/products")]
        public async Task<ActionResult<List<ProductCreateDto>>> SyncProducts()
        {
            try
            {
                _logger.LogInformation("Starting Bokun product sync");
                var addedProducts = await _service.SyncAsync();
                _logger.LogInformation("Bokun sync completed. {ProductCount} products synced", addedProducts.Count);
                return Ok(addedProducts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during Bokun sync");
                return StatusCode(500, "An error occurred during sync");
            }
        }

        [HttpGet("availability/{productId}")]
        public async Task<ActionResult<bool>> CheckAvailability(string productId)
        {
            try
            {
                // Mock availability check - replace with actual logic
                var isAvailable = !string.IsNullOrEmpty(productId);
                await Task.Delay(1000); // Simulate async work
                return Ok(isAvailable);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking availability for product {ProductId}", productId);
                return StatusCode(500, false);
            }
        }

        [HttpPost("payments/{productId}")]
        public async Task<ActionResult<bool>> ProcessPayment(string productId)
        {
            try
            {
                // Mock payment processing - replace with actual logic
                var paymentSuccess = !string.IsNullOrEmpty(productId);
                await Task.Delay(100); // Simulate async work
                return Ok(paymentSuccess);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing payment for product {ProductId}", productId);
                return StatusCode(500, false);
            }
        }
    }
}