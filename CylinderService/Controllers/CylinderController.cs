using CylinderService.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace CylinderService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // default
    public class CylinderController : ControllerBase
    {
        private readonly CylinderService.Services.IServices.ICylinder _cylinderService;

        public CylinderController(CylinderService.Services.IServices.ICylinder cylinderService)
        {
            _cylinderService = cylinderService;
        }

        // ================= READ (PUBLIC) =================
        [AllowAnonymous]
        [HttpGet("all")]
        public async Task<IActionResult> GetAllCylinders()
        {
            var cylinders = await _cylinderService.GetAllCylindersAsync();
            return Ok(cylinders);
        }

        [AllowAnonymous]
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetCylinderById(Guid id)
        {
            var cylinder = await _cylinderService.GetCylinderByIdAsync(id);
            if (cylinder is null) return NotFound();
            return Ok(cylinder);
        }

        // ================= ADMIN ONLY =================
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddCylinder([FromBody] AddUpdateCylinderDto cylinderDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var created = await _cylinderService.AddCylinderAsync(cylinderDto);
            return CreatedAtAction(nameof(GetCylinderById), new { id = created.Id }, created);
        }

        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateCylinder(Guid id, [FromBody] AddUpdateCylinderDto cylinderDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var updated = await _cylinderService.UpdateCylinderAsync(id, cylinderDto);
            if (updated is null) return NotFound();

            return Ok(updated);
        }

      
        [HttpPut("{id:guid}/daily-sales")]
        [Authorize(Roles = "Staff,Admin")]
        public async Task<IActionResult> UpdateDailySales(Guid id, [FromBody] UpdateDailySalesDto dto)
        {
            var staffId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (staffId == null)
                return Unauthorized();

            var updated = await _cylinderService.UpdateDailySalesAsync(
                id,
                staffId,
                dto.QuantitySoldToday
            );

            if (updated == null)
                return NotFound();

            return Ok(updated);
        }



        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCylinder(Guid id)
        {
            var ok = await _cylinderService.DeleteCylinderAsync(id);
            return ok ? NoContent() : NotFound();
        }
    }
}
