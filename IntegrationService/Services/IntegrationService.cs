using IntegrationService.Models;
using IntegrationService.Models.DTOs;
using IntegrationService.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace IntegrationService.Services
{
    public class IntegrationService : IIntegrationService
    {
        private readonly IAdapterRepository _repo;
        private readonly IAdapterClient _client;

        public IntegrationService(IAdapterRepository repo, IAdapterClient client)
        {
            _repo = repo;
            _client = client;
        }

        public async Task<SyncResultDto> SyncAllAdaptersAsync()
        {
            var adapters = await _repo.ListAsync(isActive: true);  // Changed to sync method
            var result = new SyncResultDto();

            foreach (var adapter in adapters)
            {
                var products = await _client.SyncProductsAsync(adapter);
                result.ProductsSynced += products.Count;
            }

            return result;
        }

        public async Task<SyncResultDto> SyncAdaptersAsync(string adapterName)
        {
            var adapter = await _repo.GetByNameAsync(adapterName);  // Changed to sync method
            var result = new SyncResultDto();

            if (adapter == null || !adapter.IsActive)
            {
                result.FailedAdapters.Add(adapterName);
                return result;
            }

            var products = await _client.SyncProductsAsync(adapter);
            result.ProductsSynced += products.Count;
            return result;
        }

        public async Task<ActionResult<AvailabilityResponseDto>> CheckAvailabilityAsync(string adapterName, AvailabilityRequestDto request)
        {
            var adapter = await _repo.GetByNameAsync(adapterName);

            if (adapter == null || !adapter.IsActive)
            {
                return new NotFoundObjectResult(new AvailabilityResponseDto
                {
                    IsAvailable = false,
                    Message = $"Adapter '{adapterName}' not found or inactive"
                });
            }

            var availability = await _client.CheckAvailabilityAsync(adapter, request);
            return new OkObjectResult(availability);
        }

        // Keep the old method for backward compatibility (marked as obsolete)
        [Obsolete("Use CheckAvailabilityAsync with AvailabilityRequestDto instead")]
        public async Task<ActionResult<bool>> AvailabilityOfProduct(string adapterName, string productId)
        {
            var request = new AvailabilityRequestDto { ProductId = productId };
            var result = await CheckAvailabilityAsync(adapterName, request);

            if (result.Result is OkObjectResult okResult && okResult.Value is AvailabilityResponseDto response)
            {
                return response.IsAvailable;
            }

            return false;
        }
    }
}