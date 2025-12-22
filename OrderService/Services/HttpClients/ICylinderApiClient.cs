using OrderService.Models.DTOs;

namespace OrderService.Services.HttpClients
{
    public interface ICylinderApiClient
    {
        Task<CylinderDto?> GetByIdAsync(Guid cylinderId);
    }
}
