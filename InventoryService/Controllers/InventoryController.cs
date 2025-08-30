using InventoryService.Models.DTOs;
using InventoryService.Services.HttpClients;
using InventoryService.Services.IService;
using Microsoft.AspNetCore.Mvc;

namespace InventoryService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InventoryController : ControllerBase
    {
        private readonly InventoryInterface _inventory;

        public InventoryController(InventoryInterface inventory)
        {
            _inventory = inventory;
        }

        // -------------------
        // Inventory endpoints
        // -------------------

        [HttpGet("all")]
        public async Task<IActionResult> GetAllInventory()
        {
            var inventories = await _inventory.GetAllInventoriesAsync();
            return Ok(inventories);
        }

        [HttpGet("{inventoryId:guid}")]
        public async Task<IActionResult> GetInventoryById(Guid inventoryId)
        {
            var inventory = await _inventory.GetInventoryByIdAsync(inventoryId);
            if (inventory == null)
                return NotFound($"Inventory with ID {inventoryId} not found.");

            return Ok(inventory);
        }

        [HttpPost]
        public async Task<IActionResult> AddInventory([FromBody] AddUpdateInventory inventoryDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var inventory = await _inventory.AddInventoryAsync(inventoryDto);
            if (inventory == null)
                return BadRequest("Inventory could not be created.");

            return CreatedAtAction(nameof(GetInventoryById), new { inventoryId = inventory.Id }, inventory);
        }

        [HttpPut("{inventoryId:guid}")]
        public async Task<IActionResult> UpdateInventory([FromBody] AddUpdateInventory updateInventory, Guid inventoryId)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var existing = await _inventory.GetInventoryByIdAsync(inventoryId);
            if (existing == null)
                return NotFound($"Inventory with ID {inventoryId} not found.");

            var updated = await _inventory.UpdateInventoryAsync(updateInventory, inventoryId);
            return Ok(updated);
        }

        [HttpDelete("{inventoryId:guid}")]
        public async Task<IActionResult> DeleteInventory(Guid inventoryId)
        {
            var inventory = await _inventory.GetInventoryByIdAsync(inventoryId);
            if (inventory == null)
                return NotFound($"Inventory with ID {inventoryId} not found.");

            var deleted = await _inventory.DeletedInventoryAsync(inventoryId);
            return deleted ? NoContent() : StatusCode(500, "Failed to delete inventory.");
        }

        // -------------------
        // Cylinder proxy endpoints (forward to CylinderService)
        // -------------------

        // GET /api/Inventory/cylinders
        [HttpGet("cylinders")]
        public async Task<IActionResult> GetCylinders()
        {
            var list = await _inventory.GetCylindersAsync();
            return Ok(list);
        }

        // POST /api/Inventory/cylinders
        [HttpPost("cylinders")]
        public async Task<IActionResult> CreateCylinder([FromBody] AddUpdateCylinderDto dto)
        {
            var created = await _inventory.CreateCylinderAsync(dto);
            return CreatedAtAction(nameof(GetCylinder), new { id = created.Id }, created);
        }

        // GET /api/Inventory/cylinders/{id}
        [HttpGet("cylinders/{id:guid}")]
        public async Task<IActionResult> GetCylinder(Guid id)
        {
            var item = await _inventory.GetCylinderByIdAsync(id);
            return item is null ? NotFound() : Ok(item);
        }

        // PUT /api/Inventory/cylinders/{id}
        [HttpPut("cylinders/{id:guid}")]
        public async Task<IActionResult> UpdateCylinder(Guid id, [FromBody] AddUpdateCylinderDto dto)
        {
            var updated = await _inventory.UpdateCylinderAsync(id, dto);
            return updated is null ? NotFound() : Ok(updated);
        }

        // DELETE /api/Inventory/cylinders/{id}
        [HttpDelete("cylinders/{id:guid}")]
        public async Task<IActionResult> DeleteCylinder(Guid id)
        {
            var ok = await _inventory.DeleteCylinderAsync(id);
            return ok ? NoContent() : NotFound();
        }

    }
}

