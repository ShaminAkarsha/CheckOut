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

        public async Task<ActionResult<bool>> AvailabilityOfProduct(string adapterName, string productId)
        {
            var adapter = await _repo.GetByNameAsync(adapterName);  // Changed to sync method

            if (adapter == null || !adapter.IsActive)
            {
               return new NotFoundResult();
            }

            var isAvailable = await _client.CheckAvailabilityAsync(adapter, productId);
            return isAvailable;
        }
    }
}