using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InventoryService.Models.DTOs;

namespace InventoryService.Services.HttpClients
{
    public interface ICylinderHttpClient
    {
        Task<IEnumerable<CylinderDto>> GetAllAsync();
        Task<CylinderDto?> GetByIdAsync(Guid id);
        Task<CylinderDto> CreateAsync(AddUpdateCylinderDto dto);
        Task<CylinderDto?> UpdateAsync(Guid id, AddUpdateCylinderDto dto);
        Task<bool> DeleteAsync(Guid id);
    }
}
