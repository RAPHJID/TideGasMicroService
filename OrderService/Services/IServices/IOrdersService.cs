using OrderService.Models.DTOs;

namespace OrderService.Services.IServices
{
    public interface IOrdersService
    {
        Task<IEnumerable<OrderReadDTO>> GetAllOrdersAsync();
        Task<OrderReadDTO?> GetOrderByIdAsync(Guid id);
        Task<OrderReadDTO> CreateOrderAsync(OrderCreateDTO dto, string token);
        Task<bool> UpdateOrderStatusAsync(Guid id, string newStatus);
        Task<bool> DeleteOrderAsync(Guid id);
    }
}
