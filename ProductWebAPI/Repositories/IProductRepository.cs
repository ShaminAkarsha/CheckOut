using ProductWebAPI.Models;

namespace ProductWebAPI.Repositories
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetAll();
        Task<Product> Get(int id);
        Task Add(Product product);
        Task Update(Product product);
        Task Delete(Product product);
        Task BulkUpsert(List<Product> products);
    }
}
