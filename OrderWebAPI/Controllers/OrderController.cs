using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using OrderWebAPI.Models;

namespace OrderWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IMongoCollection<Order> _orderCollection;
        public OrderController()
        {
            var dbHost = Environment.GetEnvironmentVariable("DB_HOST");
            var dbName = Environment.GetEnvironmentVariable("DB_NAME");
            var connectionString = $"mongodb://{dbHost}:27017/{dbName}?authSource=admin";

            var mongoUrl = MongoUrl.Create(connectionString);
            var mongoClient = new MongoClient(mongoUrl);
            var database = mongoClient.GetDatabase(mongoUrl.DatabaseName);
            _orderCollection = database.GetCollection<Order>("Order");
        }

        [HttpGet]
        public async Task<IEnumerable<Order>> GetAll()
        {
            return await _orderCollection.Find(Builders<Order>.Filter.Empty).ToListAsync();
        }

        [HttpGet("{orderId}")]
        public async Task<ActionResult<Order>> GetById(String orderId)
        {
            var filteredDefinition = Builders<Order>.Filter.Eq(x => x.OrderId, orderId);
            return await _orderCollection.Find(filteredDefinition).SingleOrDefaultAsync();
        }

        [HttpPost]
        public async Task<ActionResult> Create(Order order)
        {
            await _orderCollection.InsertOneAsync(order);
            return Ok("Order Created");
        }

        [HttpPut]
        public async Task<ActionResult> Update(Order order)
        {
            var filteredDefinition = Builders<Order>.Filter.Eq(x => x.OrderId, order.OrderId);
            await _orderCollection.ReplaceOneAsync(filteredDefinition, order);
            return Ok($"Order Updated {order.OrderId}");
        }

        [HttpDelete("{orderId}")]
        public async Task<ActionResult> Delete(String orderId)
        {
            var filteredDefinition = Builders<Order>.Filter.Eq(x => x.OrderId, orderId);
            await _orderCollection.DeleteOneAsync(filteredDefinition);
            return Ok("Order Deleted");
        }
    }
}
