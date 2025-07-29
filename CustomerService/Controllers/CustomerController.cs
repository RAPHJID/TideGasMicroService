using CustomerService.Models.DTOs;
using CustomerService.Services.IServices;
using Microsoft.AspNetCore.Mvc;

namespace CustomerService.Controllers
{
    public class CustomerController : ControllerBase
    {
        private readonly ICustomer _customerService;

        public CustomerController(ICustomer customerService)
        {
            _customerService = customerService;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllCustomers()
        {
            var customers = await _customerService.GetAllCustomersAsync();
            return Ok(customers);
        }

        [HttpGet("{customerId}")]
        public async Task<IActionResult> GetCustomerById(Guid customerId)
        { 
         var customer = await _customerService.GetCustomerByIdAsync(customerId);
            if (customer == null)
            {
                return NotFound();
            }
            return Ok(customer);
        }
        [HttpPost]
        public async Task<IActionResult> AddCustomer(CustomerDto customerDto)
        {
            var createdCustomer = await _customerService.AddCustomerAsync(customerDto);

            if (createdCustomer == null)
            {
                return BadRequest("Failed to create customer.");
            }

            return CreatedAtAction(nameof(GetCustomerById), new { id = createdCustomer.Id }, createdCustomer);
        }

        [HttpPut("{customerId}")]
        public async Task<IActionResult> UpdateCustomer(Guid customerId, [FromBody] CustomerDto customerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var updatedCustomer = await _customerService.UpdateCustomerAsync(customerId, customerDto);
            if (updatedCustomer == null)
            {
                return NotFound();
            }
            return Ok(updatedCustomer);
        }
        [HttpDelete("{customerId}")]
        public async Task<IActionResult> DeleteCustomer(Guid customerId)
        {
            var isDeleted = await _customerService.DeleteCustomerAsync(customerId);
            if (!isDeleted)
            {
                return NotFound();
            }
            return NoContent();

        }
}
    }
