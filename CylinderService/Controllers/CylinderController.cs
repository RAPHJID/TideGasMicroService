using CylinderService.Models.DTOs;
using CylinderService.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[ApiController]
[Route("api/[controller]")]
[Authorize] // default: authenticated users
public class CylinderController : ControllerBase
{
    private readonly ICylinder _cylinderService;

    public CylinderController(ICylinder cylinderService)
    {
        _cylinderService = cylinderService;
    }

    // ===== PUBLIC READ =====
    [AllowAnonymous]
    [HttpGet("all")]
    public async Task<IActionResult> GetAllCylinders()
    {
        return Ok(await _cylinderService.GetAllCylindersAsync());
    }

    [AllowAnonymous]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetCylinderById(Guid id)
    {
        var result = await _cylinderService.GetCylinderByIdAsync(id);
        return result == null ? NotFound() : Ok(result);
    }

    // ===== ADMIN ONLY =====
    [Authorize(Policy = "AdminOnly")]
    [HttpPost]
    public async Task<IActionResult> AddCylinder(AddUpdateCylinderDto dto)
    {
        return Ok(await _cylinderService.AddCylinderAsync(dto));
    }

    [Authorize(Policy = "AdminOnly")]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateCylinder(Guid id, AddUpdateCylinderDto dto)
    {
        var result = await _cylinderService.UpdateCylinderAsync(id, dto);
        return result == null ? NotFound() : Ok(result);
    }

    // ===== STAFF + ADMIN =====
    [Authorize(Policy = "AdminOrStaff")]
    [HttpPut("{id:guid}/daily-sales")]
    public async Task<IActionResult> UpdateDailySales(Guid id, UpdateDailySalesDto dto)
    {
        var staffId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (staffId == null) return Unauthorized();

        var result = await _cylinderService.UpdateDailySalesAsync(id, staffId, dto.QuantitySoldToday);
        return result == null ? NotFound() : Ok(result);
    }

    // ===== ADMIN ONLY =====
    [Authorize(Policy = "AdminOnly")]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteCylinder(Guid id)
    {
        return await _cylinderService.DeleteCylinderAsync(id)
            ? NoContent()
            : NotFound();
    }
}
