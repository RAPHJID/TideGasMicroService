using CustomerService.Models.DTOs;

namespace CustomerService.Services.IServices
{
    public interface ICustomer
    {
        Task<IEnumerable<CustomerDto>> GetAllCustomersAsync();
        Task<CustomerDto?> GetCustomerByIdAsync(Guid customerId);

        Task<CustomerDto> AddCustomerAsync(CustomerDto customerDto);
        Task<CustomerDto> UpdateCustomerAsync(Guid customerId, CustomerDto customerDto);

        Task<bool> DeleteCustomerAsync(Guid customerId);
    }
}
