using HandyCraftsAdapterWebAPI.Models;
using HandyCraftsAdapterWebAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace HandyCraftsAdapterWebAPI.Controllers
{
    [Route("api/adapter")]
    [ApiController]
    public class HandyCraftsAdapter : ControllerBase
    {
        private readonly HandyCraftSyncService _service;
        private readonly ILogger<HandyCraftsAdapter> _logger;


        public HandyCraftsAdapter(HandyCraftSyncService service, ILogger<HandyCraftsAdapter> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpPost("sync/products")]
        public async Task<ActionResult<List<ProductCreateDto>>> SyncHandyCrafts()
        {
            var addedProducts = await _service.SyncAsync();
            return Ok(addedProducts);
        }


        [HttpGet("availability/{productId}")]
        public async Task<ActionResult<bool>> CheckAvailability(string productId)
        {
            try
            {
                // Mock availability check - replace with actual logic
                var isAvailable = !string.IsNullOrEmpty(productId);
                await Task.Delay(500); // Simulate async work

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
                await Task.Delay(500); // Simulate async work

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
