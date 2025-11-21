using IntegrationService.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace IntegrationService.Services
{
    public interface IIntegrationService
    {
        Task<SyncResultDto> SyncAllAdaptersAsync();
        Task<SyncResultDto> SyncAdaptersAsync(string adapterName);
        Task<ActionResult<AvailabilityResponseDto>> CheckAvailabilityAsync(string adapterName, AvailabilityRequestDto request);
    }
}
