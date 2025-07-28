using AutoMapper;
using CustomerService.Data;
using CustomerService.Models;
using CustomerService.Models.DTOs;
using CustomerService.Services.IServices;
using Microsoft.EntityFrameworkCore;

namespace CustomerService.Services
{
    public class CustomersService : ICustomer
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public CustomersService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CustomerDto>> GetAllCustomersAsync()
        {
            var customers = await _context.Customers.ToListAsync();
            return _mapper.Map<IEnumerable<CustomerDto>>(customers);
        }
        public async Task<CustomerDto?> GetCustomerByIdAsync(Guid customerId)
        {
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.Id == customerId);
            if (customer == null) return null;
            return _mapper.Map<CustomerDto>(customer);
        }
        public async Task<CustomerDto> AddCustomerAsync(CustomerDto customerDto)
        {
            var customer = _mapper.Map<Customer>(customerDto);
            await _context.Customers.AddAsync(customer);
            await _context.SaveChangesAsync();
            return _mapper.Map<CustomerDto>(customer);
        }
        public async Task<CustomerDto> UpdateCustomerAsync(Guid customerId, CustomerDto customerDto)
        {
            var customer = await _context.Customers.FindAsync(customerId);
            if (customer == null) return null;
            _mapper.Map(customerDto, customer);
            await _context.SaveChangesAsync();
            return _mapper.Map<CustomerDto>(customer);
        }
        public async Task<bool> DeleteCustomerAsync(Guid customerId)
        {
            var customer = await _context.Customers.FindAsync(customerId);
            if (customer == null) return false;
            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
