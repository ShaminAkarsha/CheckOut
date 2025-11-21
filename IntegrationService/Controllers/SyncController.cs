using IntegrationService.Models.DTOs;
using IntegrationService.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IntegrationService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SyncController : ControllerBase
    {
        private readonly IIntegrationService _integrationService;

        public SyncController(IIntegrationService integrationService)
        {
            _integrationService = integrationService;
        }

        // POST: api/integration/sync
        [HttpPost]
        public async Task<ActionResult<SyncResultDto>> SyncAll()
        {
            var result = await _integrationService.SyncAllAdaptersAsync();
            return Ok(result);
        }

        // POST: api/integration/sync/{adapterName}
        [HttpPost("{adapterName}")]
        public async Task<ActionResult<SyncResultDto>> SyncSingle(string adapterName)
        {
            var result = await _integrationService.SyncAdaptersAsync(adapterName);
            return Ok(result);
        }
    }
}
