using OrderService.Models.DTOs;
using InventoryService.Common;

namespace OrderService.Services.IServices
{
        public interface IInventoryApiClient
        {
            Task<Result<bool>> CheckStockAsync(Guid cylinderId, int quantity);
            Task<Result<bool>> DecreaseStockAsync(Guid cylinderId, int quantity);
        }

    

}
