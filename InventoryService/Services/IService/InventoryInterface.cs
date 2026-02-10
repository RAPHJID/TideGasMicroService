using InventoryService.Common;
using InventoryService.Models.DTOs;

namespace InventoryService.Services.IService
{
    public interface IInventoryService
    {
        // ========= READ =========
        Task<IEnumerable<InventoryDto>> GetAllInventoriesAsync();
        Task<Result<InventoryDto>> GetInventoryByCylinderIdAsync(Guid cylinderId);

        // ========= WRITE =========
        Task<Result<bool>> CreateInventoryAsync(Guid cylinderId, decimal initialQuantity);

        Task<Result<InventoryDto>> AdjustInventoryAsync(
            Guid cylinderId,
            decimal quantityChange,
            string updatedByUserId
        );

        // ========= VALIDATION =========
        Task<Result<bool>> CheckStockAsync(Guid cylinderId, decimal requiredQuantity);

        // ========= ADMIN =========
        Task<Result<bool>> DeleteInventoryAsync(Guid cylinderId);
    }
}
