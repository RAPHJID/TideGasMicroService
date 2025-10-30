using InventoryService.Models;
using InventoryService.Models.DTOs;
using InventoryService.Services.IService;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;


[ApiController]
[Route("api/[controller]")]
public class InventoryController : ControllerBase
{
    private readonly InventoryInterface _inventoryService;

    public InventoryController(InventoryInterface inventoryService)
    {
        _inventoryService = inventoryService;
    }

    // GET: api/Inventory
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var inventories = await _inventoryService.GetAllInventoriesAsync();
        return Ok(inventories);
    }

    // GET: api/Inventory/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var inventory = await _inventoryService.GetInventoryByIdAsync(id);
        if (inventory == null)
            return NotFound();

        return Ok(inventory);
    }

    // POST: api/Inventory
    [HttpPost]
    public async Task<IActionResult> Add([FromBody] AddUpdateInventory dto)
    {
        if (dto == null)
            return BadRequest("Invalid inventory data.");

        await _inventoryService.AddInventoryAsync(dto);
        return Ok(new { Message = "Inventory added successfully" });
    }

    // PATCH: api/Inventory/{id}/increase/{quantity}
    [HttpPatch("{id}/increase/{quantity}")]
    public async Task<IActionResult> IncreaseQuantity(Guid id, int quantity)
    {
        if (quantity <= 0)
            return BadRequest("Quantity must be greater than zero.");

        await _inventoryService.IncreaseQuantityAsync(id, quantity);
        return Ok(new { Message = "Quantity updated successfully" });
    }

    [HttpPatch("{id}/decrease/{quantity}")]
    public async Task<IActionResult> DecreaseQuantity(Guid id, int quantity)
    {
        if (quantity <= 0)
            return BadRequest("Quantity must be greater than zero.");

        try
        {
            await _inventoryService.DecreaseQuantityAsync(id, quantity);
            return Ok(new { message = "Quantity decreased successfully" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }



    // DELETE: api/Inventory/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var inventory = await _inventoryService.GetInventoryByIdAsync(id);
        if (inventory == null)
            return NotFound();

        await _inventoryService.DeletedInventoryAsync(id);
        return Ok(new { Message = "Inventory deleted successfully" });
    }
}
