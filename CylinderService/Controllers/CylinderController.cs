using System;
using System.Threading.Tasks;
using CylinderService.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace CylinderService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CylinderController : ControllerBase
    {
        private readonly CylinderService.Services.IServices.ICylinder _cylinderService;

        // Use fully-qualified interface type here to match DI registration exactly.
        public CylinderController(CylinderService.Services.IServices.ICylinder cylinderService)
        {
            _cylinderService = cylinderService;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllCylinders()
        {
            var cylinders = await _cylinderService.GetAllCylindersAsync();
            return Ok(cylinders);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetCylinderById(Guid id)
        {
            var cylinder = await _cylinderService.GetCylinderByIdAsync(id);
            if (cylinder is null) return NotFound();
            return Ok(cylinder);
        }

        [HttpPost]
        public async Task<IActionResult> AddCylinder([FromBody] AddUpdateCylinderDto cylinderDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var created = await _cylinderService.AddCylinderAsync(cylinderDto);
            return CreatedAtAction(nameof(GetCylinderById), new { id = created.Id }, created);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateCylinder(Guid id, [FromBody] AddUpdateCylinderDto cylinderDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var updated = await _cylinderService.UpdateCylinderAsync(id, cylinderDto);
            if (updated is null) return NotFound();

            return Ok(updated);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteCylinder(Guid id)
        {
            var ok = await _cylinderService.DeleteCylinderAsync(id);
            return ok ? NoContent() : NotFound();
        }
    }
}
