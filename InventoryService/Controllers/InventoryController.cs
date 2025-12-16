using InventoryService.Models.DTOs;
using InventoryService.Services.IService;
using Microsoft.AspNetCore.Mvc;

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
        var items = await _inventoryService.GetAllInventoriesAsync();
        return Ok(items);
    }

   

    [HttpGet("{cylinderId:guid}")]
    public async Task<IActionResult> GetById(Guid cylinderId)
    {
        var inventory = await _inventoryService.GetInventoryByIdAsync(cylinderId);

        if (inventory == null)
            return NotFound();

        return Ok(inventory);
    }


    // GET: api/Inventory/{cylinderId}/check-stock?quantity=3
    [HttpGet("{cylinderId}/check-stock")]
    public async Task<IActionResult> CheckStock(Guid cylinderId, [FromQuery] int quantity)
    {
        if (quantity <= 0)
            return BadRequest(false);

        var inventory = await _inventoryService.GetInventoryByIdAsync(cylinderId);

        if (inventory == null)
            return Ok(false);

        var enough = inventory.QuantityAvailable >= quantity;

        return Ok(enough);
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

    

    [HttpPatch("{cylinderId}/increase")]
    public async Task<IActionResult> Increase(Guid cylinderId, [FromQuery] int quantity)
    {
        var result = await _inventoryService.IncreaseQuantityAsync(cylinderId, quantity);

        if (!result.IsSuccess)
            return BadRequest(result.Error);

        return Ok(new { message = "Quantity increased successfully" });
    }


    [HttpPatch("{cylinderId}/decrease")]
    public async Task<IActionResult> Decrease(Guid cylinderId, [FromQuery] int quantity)
    {
        var result = await _inventoryService.DecreaseQuantityAsync(cylinderId, quantity);

        if (!result.IsSuccess)
            return BadRequest(result.Error);

        return Ok(new { message = "Quantity decreased successfully" });
    }



    // DELETE: api/Inventory/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var item = await _inventoryService.GetInventoryByIdAsync(id);
        if (item == null)
            return NotFound();

        await _inventoryService.DeleteInventoryAsync(id);
        return Ok(new { Message = "Inventory deleted successfully" });
    }
}
