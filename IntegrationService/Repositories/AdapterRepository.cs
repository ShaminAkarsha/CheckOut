using IntegrationService.Models;
using Microsoft.EntityFrameworkCore;

namespace IntegrationService.Repositories
{
    public class AdapterRepository : IAdapterRepository
    {
        private readonly IntegrationDbContext _db;

        public AdapterRepository(IntegrationDbContext db)
        {
            _db = db;
        }

        public async Task<Adapter?> GetByIdAsync(int id, CancellationToken ct = default)
            => await _db.Adapters.FirstOrDefaultAsync(a => a.AdapterId == id, ct);

        public async Task<Adapter?> GetByNameAsync(string name, CancellationToken ct = default)
        {
            var lowered = name.Trim().ToLowerInvariant();
            return await _db.Adapters.FirstOrDefaultAsync(a => a.AdapterName.ToLower() == lowered, ct);
        }

        //public async Task<IEnumerable<Adapter>> GetAllAsync(CancellationToken ct = default)
        //{
        //    return await _db.Adapters.ToListAsync(ct);
        //}

        public async Task<IReadOnlyList<Adapter>> ListAsync(bool? isActive = null, CancellationToken ct = default)
        {
            IQueryable<Adapter> q = _db.Adapters;
            if (isActive.HasValue) q = q.Where(a => a.IsActive == isActive.Value);
            return await q.OrderBy(a => a.AdapterName).ToListAsync(ct);
        }

        public async Task<Adapter> AddAsync(Adapter adapter, CancellationToken ct = default)
        {
            _db.Adapters.Add(adapter);
            await _db.SaveChangesAsync(ct);
            return adapter;
        }

        public async Task<Adapter> UpdateAsync(Adapter adapter, CancellationToken ct = default)
        {
            _db.Adapters.Update(adapter);
            await _db.SaveChangesAsync(ct);
            return adapter;
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
        {
            var existing = await _db.Adapters.FirstOrDefaultAsync(a => a.AdapterId == id, ct);
            if (existing == null) return false;
            _db.Adapters.Remove(existing);
            await _db.SaveChangesAsync(ct);
            return true;
        }

        public async Task<bool> SetActiveAsync(int id, bool isActive, CancellationToken ct = default)
        {
            var existing = await _db.Adapters.FirstOrDefaultAsync(a => a.AdapterId == id, ct);
            if (existing == null) return false;
            existing.IsActive = isActive;
            await _db.SaveChangesAsync(ct);
            return true;
        }
    }
}