using CylinderService.Models.DTOs;
using CylinderService.Services.IServices;
using Microsoft.AspNetCore.Mvc;

namespace CylinderService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CylinderController : ControllerBase
    {
        private readonly ICylinder _cylinderService;

        public CylinderController(ICylinder cylinderService)
        {
            _cylinderService = cylinderService;
        }   

        [HttpGet("all")]
        public async Task<IActionResult> GetAllCylinders()
        {
            var cylinders = await _cylinderService.GetAllCylindersAsync();
            return Ok(cylinders);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCylinderById(Guid id)
        {
            var cylinder = await _cylinderService.GetCylinderByIdAsync(id);
            if (cylinder == null)
            {
                return NotFound();
            }
            return Ok(cylinder);
        }

        [HttpPost]
        public async Task<IActionResult> AddCylinder(AddUpdateCylinderDto cylinderDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var createdCylinder = await _cylinderService.AddCylinderAsync(cylinderDto);
            return CreatedAtAction(nameof(GetCylinderById), new { id = createdCylinder.Id }, createdCylinder);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCylinder(Guid id, AddUpdateCylinderDto cylinderDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var updatedCylinder = await _cylinderService.UpdateCylinderAsync(id, cylinderDto);
            if (updatedCylinder == null)
            {
                return NotFound();
            }
            return Ok(updatedCylinder);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCylinder(Guid id)
        {
            var result = await _cylinderService.DeleteCylinderAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }

    }
}
