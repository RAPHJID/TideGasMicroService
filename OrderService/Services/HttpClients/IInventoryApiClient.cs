using OrderService.Models.DTOs;

namespace OrderService.Services.IServices
{
    public interface IInventoryApiClient
    {
        Task<CylinderDto?> GetCylinderByIdAsync(Guid cylinderId);
        Task<bool> CheckStockAsync(Guid cylinderId, int quantity);
    }
}
