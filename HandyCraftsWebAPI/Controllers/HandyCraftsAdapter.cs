using HandyCraftsAdapterWebAPI.Models;
using HandyCraftsAdapterWebAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HandyCraftsAdapterWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HandyCraftsAdapter : ControllerBase
    {
        private readonly HandyCraftSyncService _service;

        public HandyCraftsAdapter(HandyCraftSyncService service)
        {
            _service = service;
        }

        [HttpPost("sync/handy-crafts")]
        public async Task<ActionResult<List<ProductCreateDto>>> SyncHandyCrafts()
        {
            var addedProducts = await _service.SyncAsync();
            return Ok(addedProducts);
        }
    }
}
