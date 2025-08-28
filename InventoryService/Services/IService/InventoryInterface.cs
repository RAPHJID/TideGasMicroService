using InventoryService.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InventoryService.Services.IService
{
    public interface InventoryInterface
    {
        // ===== INVENTORY METHODS =====
        Task<List<InventoryDto>> GetAllInventoriesAsync();
        Task<InventoryDto> GetInventoryByIdAsync(Guid inventoryId);
        Task<InventoryDto> AddInventoryAsync(AddUpdateInventory inventoryDto);
        Task<InventoryDto> UpdateInventoryAsync(AddUpdateInventory updatedInventory, Guid inventoryId);
        Task<bool> DeletedInventoryAsync(Guid inventoryId);

        // ===== CYLINDER METHODS (proxy to CylinderService) =====
        Task<IEnumerable<CylinderDto>> GetCylindersAsync();
        Task<CylinderDto?> GetCylinderByIdAsync(Guid id);
        Task<CylinderDto> CreateCylinderAsync(AddUpdateCylinderDto dto);
        Task<CylinderDto?> UpdateCylinderAsync(Guid id, AddUpdateCylinderDto dto);
        Task<bool> DeleteCylinderAsync(Guid id);
    }
}
