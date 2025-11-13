using CustomerWebAPI.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CustomerWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly CustomerDbContext _customerDbContext;
        public CustomerController(CustomerDbContext customerDbContext) {
            _customerDbContext = customerDbContext;
        }

        [HttpGet]
        [Authorize]
        public ActionResult<IEnumerable<Customer>> GetCustomers()
        {
            return _customerDbContext.customers;
        }

        [HttpGet("{customerId:int}")]
        public async Task<ActionResult<Customer>> GetById(int customerId)
        {
            var customer = await _customerDbContext.customers.FindAsync(customerId);
            return customer;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Create(Customer newCustomer)
        {
            await _customerDbContext.customers.AddAsync(newCustomer);
            await _customerDbContext.SaveChangesAsync();
            return Ok("New Customer added");
        }

        [HttpPut]
        [Authorize(Roles = "Admin, User")]
        public async Task<ActionResult> Update(Customer customer)
        {
            _customerDbContext.customers.Update(customer);
            await _customerDbContext.SaveChangesAsync();
            return Ok("Customer info updated");
        }

        [HttpDelete("{customerId:int}")]
        public async Task<ActionResult> Delete(int customerId)
        {
            var customer = await _customerDbContext.customers.FindAsync(customerId);
            _customerDbContext.customers.Remove(customer);
            await _customerDbContext.SaveChangesAsync();
            return Ok("Customer Removed");
        }

    }
}
