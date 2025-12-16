using InventoryService.Common;
using InventoryService.Models.DTOs;

namespace InventoryService.Services.IService;

public interface InventoryInterface
{
    Task<IEnumerable<InventoryDto>> GetAllInventoriesAsync();
    Task<Result<InventoryDto>> GetInventoryByIdAsync(Guid cylinderId);
    Task AddInventoryAsync(AddUpdateInventory dto);
    Task<Result<bool>> IncreaseQuantityAsync(Guid cylinderId, int quantity);
    Task<Result<bool>> DecreaseQuantityAsync(Guid cylinderId, int quantity);
    Task DeleteInventoryAsync(Guid cylinderId);
    Task<Result<bool>> CheckStockAsync(Guid cylinderId, int quantity);
}
