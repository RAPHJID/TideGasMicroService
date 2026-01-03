using OrderService.Models.DTOs;

namespace OrderService.Services.HttpClients
{
    public interface ICylinderApiClient
    {
        Task<CylinderDto?> GetCylinderByIdAsync(Guid id);

    }
}
