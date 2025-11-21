using IntegrationService.Models;

namespace IntegrationService.Services
{
    public interface IAdapterClient
    {
        Task<List<ProductCreateDto>> SyncProductsAsync(Adapter adapter);
        Task<bool> CheckAvailabilityAsync(Adapter adapter, string productId);
        Task<bool> ProcessPaymentsAsync(Adapter adapter, string productId);
    }
}
