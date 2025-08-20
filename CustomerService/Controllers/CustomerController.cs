using CustomerService.Models.DTOs;
using CustomerService.Services.IServices;
using Microsoft.AspNetCore.Mvc;

namespace CustomerService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomer _customerService;

        public CustomerController(ICustomer customerService)
        {
            _customerService = customerService;
        }

        // GET: api/customer
        [HttpGet]
        public async Task<IActionResult> GetAllCustomers()
        {
            var customers = await _customerService.GetAllCustomersAsync();
            return Ok(customers);
        }

        // GET: api/customer/{customerId}
        [HttpGet("{customerId}", Name = "GetCustomerById")]
        public async Task<IActionResult> GetCustomerById(Guid customerId)
        {
            var customer = await _customerService.GetCustomerByIdAsync(customerId);
            if (customer == null) return NotFound();
            return Ok(customer);
        }

        // POST: api/customer
        [HttpPost]
        public async Task<IActionResult> AddCustomer([FromBody] AddCustomerDto addDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var createdCustomer = await _customerService.AddCustomerAsync(addDto);
            if (createdCustomer == null)
                return BadRequest("Failed to create customer.");

            // Return 201 with Location header to GET endpoint
            return CreatedAtRoute("GetCustomerById", new { customerId = createdCustomer.Id }, createdCustomer);
        }

        // PUT: api/customer/{customerId}
        [HttpPut("{customerId}")]
        public async Task<IActionResult> UpdateCustomer(Guid customerId, [FromBody] UpdateCustomerDto updateDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var updatedCustomer = await _customerService.UpdateCustomerAsync(customerId, updateDto);
            if (updatedCustomer == null) return NotFound();

            return Ok(updatedCustomer);
        }

        // DELETE: api/customer/{customerId}
        [HttpDelete("{customerId}")]
        public async Task<IActionResult> DeleteCustomer(Guid customerId)
        {
            var deleted = await _customerService.DeleteCustomerAsync(customerId);
            if (!deleted) return NotFound();

            return NoContent();
        }
    }
}
