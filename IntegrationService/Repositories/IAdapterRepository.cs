using IntegrationService.Models;

namespace IntegrationService.Repositories
{
    public interface IAdapterRepository
    {
        Task<Adapter?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<Adapter?> GetByNameAsync(string name, CancellationToken ct = default);
        //Task<IEnumerable<Adapter>> GetAllAsync(CancellationToken ct = default);
        Task<IReadOnlyList<Adapter>> ListAsync(bool? isActive = null, CancellationToken ct = default);
        Task<Adapter> AddAsync(Adapter adapter, CancellationToken ct = default);
        Task<Adapter> UpdateAsync(Adapter adapter, CancellationToken ct = default);
        Task<bool> DeleteAsync(int id, CancellationToken ct = default);
        Task<bool> SetActiveAsync(int id, bool isActive, CancellationToken ct = default);
    }
}
