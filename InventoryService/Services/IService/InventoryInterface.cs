using InventoryService.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InventoryService.Services.IService;

public interface InventoryInterface
{
    // Inventory methods
    Task<List<InventoryDto>> GetAllInventoriesAsync();
    Task<ServiceResult<InventoryDto>> GetInventoryByIdAsync(Guid inventoryId);
    Task<ServiceResult<InventoryDto>> AddInventoryAsync(AddUpdateInventory inventoryDto);
    Task<ServiceResult<InventoryDto>> UpdateInventoryAsync(AddUpdateInventory updatedInventory, Guid inventoryId);
    Task<bool> CheckStockAsync(Guid cylinderId, int quantity);

    Task DecreaseQuantityAsync(Guid id, int quantity);
    Task IncreaseQuantityAsync(Guid id, int quantity);

    Task<ServiceResult<bool>> DeletedInventoryAsync(Guid inventoryId);

    // Cylinder proxy methods
    Task<IEnumerable<CylinderDto>> GetCylindersAsync();
    Task<CylinderDto?> GetCylinderByIdAsync(Guid id);
    Task<CylinderDto> CreateCylinderAsync(AddUpdateCylinderDto dto);
    Task<CylinderDto?> UpdateCylinderAsync(Guid id, AddUpdateCylinderDto dto);
    Task<bool> DeleteCylinderAsync(Guid id);
}
