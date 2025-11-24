using Microsoft.EntityFrameworkCore;
using ProductWebAPI.Dtos;
using ProductWebAPI.http;
using ProductWebAPI.Models;

namespace ProductWebAPI.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ProductDbContext _db;
        private readonly IntegrationApiClient _http;

        public ProductRepository(ProductDbContext db, IntegrationApiClient http)
        {
            _db = db;
            _http = http;
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

        public async Task<ProductAvailabilityResponseDto> GetAvailability(ProductAvailabilityRequestDto availabilityDto)
        {
            int id = availabilityDto.ProductId;
            var product = await _db.Products.FindAsync(id);
            var result = new ProductAvailabilityResponseDto();
            if (product == null)
            {
                result.IsAvailable = false;
                result.Message = "Product not found";
                return result;
            }
            var request = new ProductAvailabilityRequestDto
            {
                ExternalId = product.ExternalId,
                CheckInDate = availabilityDto.CheckInDate,
                CheckOutDate = availabilityDto.CheckOutDate,
                NumberOfGuests = availabilityDto.NumberOfGuests > 0 ? availabilityDto.NumberOfGuests : 1,
                Quantity = availabilityDto.Quantity
            };
            var availability = await _http.CheckAvailability(request, product.Source);   
            return availability;
        }
    }
}
