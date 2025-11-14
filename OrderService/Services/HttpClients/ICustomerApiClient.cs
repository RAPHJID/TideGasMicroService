using OrderService.Models.DTOs;

namespace OrderService.Services.HttpClients
{
    public interface ICustomerApiClient
    {
        Task<CustomerDto?> GetCustomerByIdAsync(Guid customerId);
    }
}
