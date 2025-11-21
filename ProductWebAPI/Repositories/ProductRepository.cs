using Microsoft.EntityFrameworkCore;
using ProductWebAPI.Models;

namespace ProductWebAPI.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ProductDbContext _db;

        public ProductRepository(ProductDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<Product>> GetAll() =>
            await _db.Products.ToListAsync();

        public async Task<Product> Get(int id) =>
            await _db.Products.FindAsync(id);

        public async Task Add(Product product)
        {
            await _db.Products.AddAsync(product);
            await _db.SaveChangesAsync();
        }

        public async Task Update(Product product)
        {
            _db.Products.Update(product);
            await _db.SaveChangesAsync();
        }

        public async Task Delete(Product product)
        {
            _db.Products.Remove(product);
            await _db.SaveChangesAsync();
        }

        public async Task BulkUpsert(List<Product> products)
        {
            foreach (var p in products)
            {
                var existing = await _db.Products
                    .FirstOrDefaultAsync(x => x.ExternalId == p.ExternalId && x.Source == p.Source);

                if (existing == null)
                    _db.Products.Add(p);
                else
                {
                    p.ProductId = existing.ProductId;

                    _db.Entry(existing).CurrentValues.SetValues(p);
                }
            }
            await _db.SaveChangesAsync();
        }
    }
}
