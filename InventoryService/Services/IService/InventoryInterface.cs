using InventoryService.Models.DTOs;

namespace InventoryService.Services.IService;

public interface InventoryInterface
{
    Task<IEnumerable<InventoryDto>> GetAllInventoriesAsync();
    Task<InventoryDto?> GetInventoryByIdAsync(Guid cylinderId);
    Task AddInventoryAsync(AddUpdateInventory dto);
    Task<bool> IncreaseQuantityAsync(Guid cylinderId, int quantity);
    Task DecreaseQuantityAsync(Guid cylinderId, int quantity);
    Task DeleteInventoryAsync(Guid cylinderId);
    Task<bool> CheckStockAsync(Guid cylinderId, int quantity);
}
