using IntegrationService.Models;
using IntegrationService.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace IntegrationService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdapterController : ControllerBase
    {
        private readonly IAdapterRepository _repository;
        private readonly ILogger<AdapterController> _logger;

        public AdapterController(IAdapterRepository repository, ILogger<AdapterController> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        // POST: api/adapter
        // Creates a new adapter using the entity directly (no DTOs/extra validation)
        [HttpPost]
        public async Task<ActionResult<Adapter>> Create([FromBody] Adapter adapter, CancellationToken ct)
        {
            var created = await _repository.AddAsync(adapter, ct);
            _logger.LogInformation("Created adapter {AdapterId}", created.AdapterId);
            return CreatedAtAction(nameof(GetById), new { id = created.AdapterId }, created);
        }

        // GET: api/adapter/{id}
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Adapter>> GetById(int id, CancellationToken ct)
        {
            var adapter = await _repository.GetByIdAsync(id, ct);
            if (adapter == null) return NotFound();
            return Ok(adapter);
        }

        // GET: api/adapter
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Adapter>>> List([FromQuery] bool? isActive, CancellationToken ct)
        {
            var list = await _repository.ListAsync(isActive, ct);
            return Ok(list);
        }

        // PUT: api/adapter/{id}
        // Updates the adapter; uses route id, no DTOs/extra validation
        [HttpPut("{id:int}")]
        public async Task<ActionResult<Adapter>> Update(int id, [FromBody] Adapter input, CancellationToken ct)
        {
            var existing = await _repository.GetByIdAsync(id, ct);
            if (existing == null) return NotFound();

            // Apply incoming values to the existing entity
            existing.AdapterName = input.AdapterName;
            existing.BaseUrl = input.BaseUrl;
            existing.ApiKey = input.ApiKey;
            existing.CustomHeadersJson = input.CustomHeadersJson;
            existing.IsActive = input.IsActive;

            var updated = await _repository.UpdateAsync(existing, ct);
            _logger.LogInformation("Updated adapter {AdapterId}", id);
            return Ok(updated);
        }

        // DELETE: api/adapter/{id}
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var success = await _repository.DeleteAsync(id, ct);
            if (!success) return NotFound();
            _logger.LogInformation("Deleted adapter {AdapterId}", id);
            return NoContent();
        }

        // PATCH: api/adapter/{id}/activate
        [HttpPatch("{id:int}/activate")]
        public async Task<IActionResult> Activate(int id, CancellationToken ct)
        {
            var success = await _repository.SetActiveAsync(id, true, ct);
            if (!success) return NotFound();
            _logger.LogInformation("Activated adapter {AdapterId}", id);
            return NoContent();
        }

        // PATCH: api/adapter/{id}/deactivate
        [HttpPatch("{id:int}/deactivate")]
        public async Task<IActionResult> Deactivate(int id, CancellationToken ct)
        {
            var success = await _repository.SetActiveAsync(id, false, ct);
            if (!success) return NotFound();
            _logger.LogInformation("Deactivated adapter {AdapterId}", id);
            return NoContent();
        }
    }
}
