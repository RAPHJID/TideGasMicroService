using InventoryService.Models.DTOs;
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

        [HttpGet("all")]
        public async Task<IActionResult> GetAllInventory()
        {
            var inventories = await _inventory.GetAllInventoriesAsync();
            return Ok(inventories);
        }

        [HttpGet("{inventoryId}")]
        public async Task<IActionResult> GetInventoryById(Guid inventoryId)
        {
            var inventory = await _inventory.GetInventoryByIdAsync(inventoryId);
            if (inventory == null)
                return NotFound($"Inventory with ID {inventoryId} not found.");

            return Ok(inventory);
        }

        [HttpPost]
        public async Task<IActionResult> AddInventory(AddUpdateInventory inventoryDto)
        {
            var inventory = await _inventory.AddInventoryAsync(inventoryDto);
            if (inventory == null)
                return BadRequest("Inventory could not be created.");

            return Ok(inventory);
        }

        [HttpPut("{inventoryId}")]
        public async Task<IActionResult> UpdateInventory(AddUpdateInventory updateInventory, Guid inventoryId)
        {
            var existing = await _inventory.GetInventoryByIdAsync(inventoryId);
            if (existing == null)
                return NotFound($"Inventory with ID {inventoryId} not found.");

            var updated = await _inventory.UpdateInventoryAsync(updateInventory, inventoryId);
            return Ok(updated);
        }

        [HttpDelete("{inventoryId}")]
        public async Task<IActionResult> DeleteInventory(Guid inventoryId)
        {
            var inventory = await _inventory.GetInventoryByIdAsync(inventoryId);
            if (inventory == null)
                return NotFound($"Inventory with ID {inventoryId} not found.");

            await _inventory.DeletedInventoryAsync(inventoryId);
            return Ok($"Inventory with ID {inventoryId} deleted successfully.");
        }
    }
}
