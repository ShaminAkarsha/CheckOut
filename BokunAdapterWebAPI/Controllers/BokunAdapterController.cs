using BokunAdapterWebAPI.Models;
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
        public async Task<ActionResult<List<ProductCreateDto>>> SyncBokun()
        {
            var addedProducts = await _service.SyncAsync();
            return Ok(addedProducts);
        }
    }
}
