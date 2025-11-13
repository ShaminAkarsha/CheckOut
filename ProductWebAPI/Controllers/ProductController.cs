using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProductWebAPI.Models;

namespace ProductWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly ProductDbContext _dbContext;
        public ProductController(ProductDbContext productDbContext)
        {
            _dbContext = productDbContext;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Product>> GetAll()
        {
            return _dbContext.Products;
        }

        [HttpGet("{productId:int}")]
        public async Task<ActionResult<Product>> Get(int productId)
        {
            var products = await _dbContext.Products.FindAsync(productId);
            return Ok(products);
        }

        [HttpPost]
        public async Task<ActionResult> Create(Product product)
        {
            await _dbContext.Products.AddAsync(product);
            await _dbContext.SaveChangesAsync();
            return Ok("Product Added");
        }

        [HttpPut]
        public async Task<ActionResult> Update(Product product)
        {
            _dbContext.Products.Update(product);
            await _dbContext.SaveChangesAsync();
            return Ok("Product Updated");
        }

        [HttpDelete("{productId:int}")]
        public async Task<ActionResult<Product>> Remove(int productId)
        {
            var products = await _dbContext.Products.FindAsync(productId);
            _dbContext.Products.Remove(products);
            await _dbContext.SaveChangesAsync();
            return Ok("Product Deleted");
        }

    }
}
