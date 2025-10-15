using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using OrderService.Models.DTOs;
using OrderService.Services.IServices;

namespace OrderService.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IOrdersService _ordersService;
        private readonly IMapper _mapper;

        public OrderController(IOrdersService ordersService, IMapper mapper)
        {
            _ordersService = ordersService;
            _mapper = mapper;
        }

        // GET: api/Order
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderReadDTO>>> GetAllOrders()
        {
            var orders = await _ordersService.GetAllOrdersAsync();
            return Ok(orders);
        }

        // GET: api/Order/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderReadDTO>> GetOrderById(Guid id)
        {
            var order = await _ordersService.GetOrderByIdAsync(id);
            if (order == null)
                return NotFound();

            return Ok(order);
        }

     


        // POST: api/Order
        [HttpPost]
        public async Task<ActionResult<OrderReadDTO>> CreateOrder([FromBody]OrderCreateDTO dto)
        {
            var newOrder = await _ordersService.CreateOrderAsync(dto);
            return CreatedAtAction(nameof(GetOrderById), new { id = newOrder.Id }, newOrder);
        }

       
        // PATCH: api/Order/{id}/status
        [HttpPatch("{id}/status")]
        public async Task<ActionResult> UpdateStatus(Guid id, [FromBody] string status)
        {
            var success = await _ordersService.UpdateOrderStatusAsync(id, status);
            if (!success)
                return NotFound();

            return NoContent();
        }

        // DELETE: api/Order/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteOrder(Guid id)
        {
            var success = await _ordersService.DeleteOrderAsync(id);
            if (!success)
                return NotFound();

            return NoContent();
        }
    }
}
