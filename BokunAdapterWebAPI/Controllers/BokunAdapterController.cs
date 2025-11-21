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

        /// <summary>
        /// Check availability for Bokun tours with booking dates and guest count
        /// </summary>
        /// <param name="request">Availability request with product ID, dates, and guest information</param>
        /// <returns>Availability status with latest pricing</returns>
        [HttpPost("availability")]
        public async Task<ActionResult<AvailabilityResponseDto>> CheckAvailability([FromBody] AvailabilityRequestDto request)
        {
            try
            {
                if (request == null || string.IsNullOrWhiteSpace(request.ProductId))
                {
                    return BadRequest(new AvailabilityResponseDto
                    {
                        IsAvailable = false,
                        Message = "Product ID is required"
                    });
                }

                _logger.LogInformation("Checking availability for Bokun tour {ProductId}", request.ProductId);

                // Mock Bokun availability logic - replace with actual Bokun API calls
                // For tours, we typically need check-in date and number of guests
                var hasRequiredBookingInfo = request.CheckInDate.HasValue && request.NumberOfGuests.HasValue;
                var isWithinValidDateRange = !request.CheckInDate.HasValue || request.CheckInDate.Value >= DateTime.Today;
                var isValidGuestCount = !request.NumberOfGuests.HasValue || request.NumberOfGuests.Value > 0;

                bool isAvailable = hasRequiredBookingInfo && isWithinValidDateRange && isValidGuestCount;
                decimal? pricePerPerson = isAvailable ? GetMockTourPrice(request.ProductId, request.CheckInDate) : null;

                await Task.Delay(100); // Simulate API call delay

                return Ok(new AvailabilityResponseDto
                {
                    IsAvailable = isAvailable,
                    LatestPricePerUnit = pricePerPerson,
                    Currency = "USD",
                    AvailableQuantity = isAvailable ? 10 : 0, // Mock available spots
                    Message = isAvailable ? "Available for booking" : GetUnavailabilityReason(request)
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking availability for product {ProductId}", request?.ProductId);
                return StatusCode(500, new AvailabilityResponseDto
                {
                    IsAvailable = false,
                    Message = "An error occurred while checking availability"
                });
            }
        }

        /// <summary>
        /// Legacy GET endpoint for backward compatibility
        /// </summary>
        [HttpGet("availability/{productId}")]
        [Obsolete("Use POST /availability with AvailabilityRequestDto for enhanced functionality")]
        public async Task<ActionResult<bool>> CheckAvailabilityLegacy(string productId)
        {
            try
            {
                // Convert to new format and call the enhanced method
                var request = new AvailabilityRequestDto
                {
                    ProductId = productId,
                    CheckInDate = DateTime.Today.AddDays(1),
                    NumberOfGuests = 2
                };

                var result = await CheckAvailability(request);
                if (result.Result is OkObjectResult okResult && okResult.Value is AvailabilityResponseDto response)
                {
                    return Ok(response.IsAvailable);
                }

                return Ok(false);
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

        /// <summary>
        /// Mock method to get tour pricing based on product and date
        /// </summary>
        private decimal GetMockTourPrice(string productId, DateTime? checkInDate)
        {
            // Mock pricing logic - replace with actual Bokun API pricing calls
            var basePrice = 150m; // Base tour price
            var seasonMultiplier = IsHighSeason(checkInDate) ? 1.5m : 1.0m;
            return basePrice * seasonMultiplier;
        }

        /// <summary>
        /// Mock method to determine if date is in high season
        /// </summary>
        private bool IsHighSeason(DateTime? date)
        {
            if (!date.HasValue) return false;
            var month = date.Value.Month;
            return month >= 6 && month <= 8; // Summer months
        }

        /// <summary>
        /// Get reason why tour is unavailable
        /// </summary>
        private string GetUnavailabilityReason(AvailabilityRequestDto request)
        {
            if (!request.CheckInDate.HasValue)
                return "Check-in date is required for tour bookings";

            if (!request.NumberOfGuests.HasValue || request.NumberOfGuests.Value <= 0)
                return "Number of guests must be specified and greater than 0";

            if (request.CheckInDate.Value < DateTime.Today)
                return "Cannot book tours for past dates";

            return "Tour not available for selected date and guest count";
        }
    }
}