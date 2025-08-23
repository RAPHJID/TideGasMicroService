using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CylinderService.Models.DTOs;

namespace CylinderService.Services.IServices
{
    public interface ICylinder
    {
        Task<IEnumerable<CylinderDto>> GetAllCylindersAsync();
        Task<CylinderDto?> GetCylinderByIdAsync(Guid id);
        Task<CylinderDto> AddCylinderAsync(AddUpdateCylinderDto cylinderDto);
        Task<CylinderDto> UpdateCylinderAsync(Guid id, AddUpdateCylinderDto cylinderDto);
        Task<bool> DeleteCylinderAsync(Guid id);
    }
}
