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

        [HttpGet("{adapterName}/{productId}")]
        public async Task<ActionResult<bool>> CeckAvailability(string adapterName, string productId)
        {
            var result = await _client.AvailabilityOfProduct(adapterName, productId);
            return Ok(result);
        }

    }
}
