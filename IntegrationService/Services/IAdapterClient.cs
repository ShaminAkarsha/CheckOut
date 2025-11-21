using IntegrationService.Models;
using IntegrationService.Models.DTOs;

namespace IntegrationService.Services
{
    public interface IAdapterClient
    {
        Task<List<ProductCreateDto>> SyncProductsAsync(Adapter adapter);
        Task<AvailabilityResponseDto> CheckAvailabilityAsync(Adapter adapter, AvailabilityRequestDto request);
        Task<bool> ProcessPaymentsAsync(Adapter adapter, string productId);
    }
}
