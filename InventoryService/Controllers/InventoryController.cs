using InventoryService.Models.DTOs;
using InventoryService.Services.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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

        // ================= READ =================

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
            return HandleResult(result);
        }

        // ================= VALIDATION =================

        [AllowAnonymous]
        [HttpGet("{cylinderId}/check-stock")]
        public async Task<IActionResult> CheckStock(Guid cylinderId, [FromQuery] decimal quantity)
        {
            if (quantity <= 0)
                return BadRequest("Quantity must be greater than zero.");

            var result = await _inventoryService.CheckStockAsync(cylinderId, quantity);
            return HandleResult(result);
        }

        // ================= ADMIN ONLY =================

        [Authorize(Policy = "AdminOnly")]
        [HttpPost("{cylinderId}")]
        public async Task<IActionResult> CreateInventory(Guid cylinderId, [FromQuery] decimal quantity)
        {
            if (quantity < 0)
                return BadRequest("Quantity cannot be negative.");

            var result = await _inventoryService.CreateInventoryAsync(cylinderId, quantity);

            if (!result.IsSuccess)
                return BadRequest(result.Error);

            return CreatedAtAction(nameof(GetById), new { cylinderId }, "Inventory created");
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpDelete("{cylinderId}")]
        public async Task<IActionResult> Delete(Guid cylinderId)
        {
            var result = await _inventoryService.DeleteInventoryAsync(cylinderId);

            if (!result.IsSuccess)
                return NotFound(result.Error);

            return NoContent();
        }

        // ================= ADMIN + STAFF =================

        [Authorize(Policy = "AdminOrStaff")]
        [HttpPatch("{cylinderId}/adjust")]
        public async Task<IActionResult> AdjustInventory(Guid cylinderId, [FromQuery] decimal quantityChange)
        {
            if (quantityChange == 0)
                return BadRequest("Quantity change cannot be zero.");

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "unknown";

            var result = await _inventoryService.AdjustInventoryAsync(
                cylinderId,
                quantityChange,
                userId
            );

            if (!result.IsSuccess)
                return BadRequest(result.Error);

            return Ok(result.Value);
        }

        // ================= HELPER =================

        private IActionResult HandleResult<T>(Common.Result<T> result)
        {
            if (!result.IsSuccess)
                return NotFound(result.Error);

            return Ok(result.Value);
        }
    }
}