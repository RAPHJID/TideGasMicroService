using InventoryService.Models.DTOs;
using InventoryService.Services.IService;
using Microsoft.AspNetCore.Authorization;
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
        var result = await _inventoryService.GetInventoryByIdAsync(cylinderId);

        if (!result.IsSuccess)
            return NotFound(result.Error);

        return Ok(result.Value);
    }

    [AllowAnonymous]
    [HttpGet("{cylinderId}/check-stock")]
    public async Task<IActionResult> CheckStock(
        Guid cylinderId,
        [FromQuery] int quantity)
    {
        if (quantity <= 0)
            return BadRequest("Quantity must be greater than zero.");

        var result = await _inventoryService.CheckStockAsync(cylinderId, quantity);

        if (!result.IsSuccess)
            return NotFound(result.Error);

        return Ok(result.Value);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Add([FromBody] AddUpdateInventory dto)
    {
        if (dto == null)
            return BadRequest("Invalid inventory data.");

        await _inventoryService.AddInventoryAsync(dto);
        return Ok(new { Message = "Inventory added successfully" });
    }

    [Authorize]
    [HttpPatch("{cylinderId}/increase/{quantity}")]
    public async Task<IActionResult> IncreaseStock(Guid cylinderId, int quantity)
    {
        var result = await _inventoryService.IncreaseQuantityAsync(cylinderId, quantity);

        if (!result.IsSuccess)
            return BadRequest(result.Error);

        return Ok(true);
    }

    [Authorize]
    [HttpPatch("{cylinderId}/decrease/{quantity}")]
    public async Task<IActionResult> DecreaseStock(Guid cylinderId, int quantity)
    {
        var result = await _inventoryService.DecreaseQuantityAsync(cylinderId, quantity);

        if (!result.IsSuccess)
            return BadRequest(result.Error);

        return Ok(true);
    }

    [Authorize] 
    [HttpDelete("{cylinderId}")]
    public async Task<IActionResult> Delete(Guid cylinderId)
    {
        var result = await _inventoryService.DeleteInventoryAsync(cylinderId);

        if (!result.IsSuccess)
            return NotFound(result.Error);

        return NoContent(); 
    }

}
