using InventoryService.Models.DTOs;
using InventoryService.Services.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventoryService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InventoryController : ControllerBase
    {
        private readonly InventoryInterface _inventoryService;

        public InventoryController(InventoryInterface inventoryService)
        {
            _inventoryService = inventoryService;
        }

        // ========= READ =========

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var items = await _inventoryService.GetAllInventoriesAsync();
            return Ok(items);
        }

        [AllowAnonymous]
        [HttpGet("{cylinderId}")]
        public async Task<IActionResult> GetById(Guid cylinderId)
        {
            var result = await _inventoryService.GetInventoryByCylinderIdAsync(cylinderId);

            if (!result.IsSuccess)
                return NotFound(result.Error);

            return Ok(result.Value);
        }

        // ========= VALIDATION =========

        [AllowAnonymous]
        [HttpGet("{cylinderId}/check-stock")]
        public async Task<IActionResult> CheckStock(Guid cylinderId, [FromQuery] decimal quantity)
        {
            var result = await _inventoryService.CheckStockAsync(cylinderId, quantity);

            if (!result.IsSuccess)
                return NotFound(result.Error);

            return Ok(result.Value);
        }

        // ========= CREATE =========

        [Authorize]
        [HttpPost("{cylinderId}")]
        public async Task<IActionResult> CreateInventory(Guid cylinderId, [FromQuery] decimal quantity)
        {
            var result = await _inventoryService.CreateInventoryAsync(cylinderId, quantity);

            if (!result.IsSuccess)
                return BadRequest(result.Error);

            return Ok("Inventory created");
        }

        // ========= ADJUST =========

        [Authorize]
        [HttpPatch("{cylinderId}/adjust")]
        public async Task<IActionResult> AdjustInventory(Guid cylinderId, [FromQuery] decimal quantityChange)
        {
            var userId = User.FindFirst("sub")?.Value;

            var result = await _inventoryService.AdjustInventoryAsync(
                cylinderId,
                quantityChange,
                userId ?? "unknown"
            );

            if (!result.IsSuccess)
                return BadRequest(result.Error);

            return Ok(result.Value);
        }

        // ========= DELETE =========

        [Authorize(Policy = "AdminOnly")]
        [HttpDelete("{cylinderId}")]
        public async Task<IActionResult> Delete(Guid cylinderId)
        {
            var result = await _inventoryService.DeleteInventoryAsync(cylinderId);

            if (!result.IsSuccess)
                return NotFound(result.Error);

            return NoContent();
        }
    }
}