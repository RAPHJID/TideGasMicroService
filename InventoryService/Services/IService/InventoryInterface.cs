using InventoryService.Models;
using InventoryService.Models.DTOs;

namespace InventoryService.Services.IService
{
    public interface InventoryInterface
    {
        Task <List<InventoryDto>> GetAllInventoriesAsync();
        Task<InventoryDto> GetInventoryByIdAsync(Guid inventoryId);

        Task<InventoryDto> AddInventoryAsync(InventoryDto inventoryDto);
        Task<InventoryDto> UpdateInventoryAsync(InventoryDto updatedInventory, Guid inventoryId);
        Task <bool> DeletedInventoryAsync(Guid inventoryId);

    }
}
