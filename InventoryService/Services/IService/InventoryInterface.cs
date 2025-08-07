using InventoryService.Models;
using InventoryService.Models.DTOs;

namespace InventoryService.Services.IService
{
    public interface InventoryInterface
    {
        Task <List<InventoryDto>> GetAllInventoriesAsync();
        Task<InventoryDto> GetInventoryByIdAsync(Guid inventoryId);

        Task<InventoryDto> AddInventoryAsync(AddUpdateInventory inventoryDto);
        Task<InventoryDto> UpdateInventoryAsync(AddUpdateInventory updatedInventory, Guid inventoryId);
        Task <bool> DeletedInventoryAsync(Guid inventoryId);

    }
}
