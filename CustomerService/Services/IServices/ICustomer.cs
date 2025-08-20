using CustomerService.Models.DTOs;

namespace CustomerService.Services.IServices
{
    public interface ICustomer
    {
        Task<IEnumerable<CustomerDto>> GetAllCustomersAsync();
        Task<CustomerDto?> GetCustomerByIdAsync(Guid customerId);
        Task<CustomerDto?> AddCustomerAsync(AddCustomerDto dto);
        Task<CustomerDto?> UpdateCustomerAsync(Guid customerId, UpdateCustomerDto dto);
        Task<bool> DeleteCustomerAsync(Guid customerId);
    }
}
