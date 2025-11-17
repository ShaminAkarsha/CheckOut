using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ProductWebAPI.Dtos;
using ProductWebAPI.Models;
using ProductWebAPI.Repositories;

namespace ProductWebAPI.Controllers
{
    [ApiController]
    [Route("api/v1/product")]
    public class ProductController : ControllerBase
    {
        private readonly IProductRepository _repo;
        private readonly IMapper _mapper;

        public ProductController(IProductRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetAll()
        {
            var products = await _repo.GetAll();
            return Ok(_mapper.Map<IEnumerable<ProductDto>>(products));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ProductDto>> Get(int id)
        {
            var product = await _repo.Get(id);
            if (product == null) return NotFound();

            return Ok(_mapper.Map<ProductDto>(product));
        }

        [HttpPost]
        public async Task<ActionResult> Create(ProductCreateDto dto)
        {
            var product = _mapper.Map<Product>(dto);
            product.SyncedAt = DateTime.UtcNow;

            await _repo.Add(product);
            return Ok("Product added");
        }

        [HttpPut]
        public async Task<ActionResult> Update(ProductUpdateDto dto)
        {
            var existing = await _repo.Get(dto.ProductId);
            if (existing == null) return NotFound();

            _mapper.Map(dto, existing);
            await _repo.Update(existing);

            return Ok("Product updated");
        }

        [HttpPost("bulk")]
        public async Task<ActionResult> BulkSync(List<ProductSyncDto> dtos)
        {
            var products = _mapper.Map<List<Product>>(dtos);

            foreach (var p in products)
                p.SyncedAt = DateTime.UtcNow;

            await _repo.BulkUpsert(products);

            return Ok("Bulk sync completed");
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Remove(int id)
        {
            var product = await _repo.Get(id);
            if (product == null) return NotFound();

            await _repo.Delete(product);
            return Ok("Product deleted");
        }
    }
}
