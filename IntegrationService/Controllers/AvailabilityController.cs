using IntegrationService.Models.DTOs;
using IntegrationService.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IntegrationService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AvailabilityController : ControllerBase
    {
        private readonly IIntegrationService _client;

      public AvailabilityController(IIntegrationService client)
{
        _client = client;
        }

        /// <summary>
        /// Check availability for a product/service with flexible request parameters
 /// </summary>
        /// <param name="adapterName">Name of the adapter to use</param>
        /// <param name="request">Availability check request with product ID and optional quantity/dates</param>
        /// <returns>Availability status with latest pricing information</returns>
        [HttpPost("{adapterName}")]
        public async Task<ActionResult<AvailabilityResponseDto>> CheckAvailability(
 string adapterName, 
    [FromBody] AvailabilityRequestDto request)
        {
   if (string.IsNullOrWhiteSpace(adapterName))
            {
                return BadRequest(new AvailabilityResponseDto
        {
      IsAvailable = false,
                    Message = "Adapter name is required"
      });
     }

          if (request == null || string.IsNullOrWhiteSpace(request.ProductId))
       {
   return BadRequest(new AvailabilityResponseDto
     {
          IsAvailable = false,
     Message = "Product ID is required"
    });
            }

 var result = await _client.CheckAvailabilityAsync(adapterName, request);
  return result;
        }

        /// <summary>
 /// Legacy endpoint - Check availability using GET method (for backward compatibility)
        /// </summary>
        /// <param name="adapterName">Name of the adapter to use</param>
        /// <param name="productId">Product ID to check</param>
        /// <returns>Simple boolean availability status</returns>
        [HttpGet("{adapterName}/{productId}")]
        [Obsolete("Use POST method with AvailabilityRequestDto for enhanced functionality")]
        public async Task<ActionResult<bool>> CheckAvailabilityLegacy(string adapterName, string productId)
        {
            var request = new AvailabilityRequestDto { ProductId = productId };
    var result = await _client.CheckAvailabilityAsync(adapterName, request);

if (result.Result is OkObjectResult okResult && okResult.Value is AvailabilityResponseDto response)
      {
      return Ok(response.IsAvailable);
          }

    return Ok(false);
        }
    }
}
