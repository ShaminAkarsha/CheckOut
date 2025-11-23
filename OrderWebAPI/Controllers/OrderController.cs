using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrderWebAPI.Models;

namespace OrderWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        [HttpGet("{userId:int}")]
        // get all orders from order db by user id

        [HttpGet("{userId:int}/{orderId:int}")]
        // fetch order by id from order db and usr id

        [HttpPost]
        // fetch cart items from cartApi 
        // save it in the order db
        // clear the cart using cartApi
        // check all items availability from productApi
        // if all items are available, confirm the order
        // return order details with ordered items 


    }
}
