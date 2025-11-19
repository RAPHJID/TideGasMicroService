using OrderService.Models.DTOs;

namespace OrderService.Services.IServices
{
    public interface IInventoryApiClient
    {
        Task<CylinderDTO?> GetCylinderByIdAsync(Guid id);
        Task<bool> CheckStockAsync(Guid cylinderId, int quantity);
    }
}
