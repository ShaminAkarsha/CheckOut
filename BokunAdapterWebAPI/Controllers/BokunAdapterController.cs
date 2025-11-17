using BokunAdapterWebAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BokunAdapterWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BokunAdapterController : ControllerBase
    {
        private readonly BokunSyncService _service;

        public BokunAdapterController(BokunSyncService service)
        {
            _service = service;
        }

        [HttpPost("sync/bokun")]
        public async Task<IActionResult> SyncBokun()
        {
            await _service.SyncAsync();
            return Ok("Bokun products synced.");
        }
    }
}
