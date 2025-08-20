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
        private readonly AppDbContext _db;
        private readonly IMapper _mapper;

        public CustomersService(AppDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CustomerDto>> GetAllCustomersAsync()
        {
            var customers = await _db.Customers.AsNoTracking().ToListAsync();
            return _mapper.Map<IEnumerable<CustomerDto>>(customers);
        }

        public async Task<CustomerDto?> GetCustomerByIdAsync(Guid customerId)
        {
            var customer = await _db.Customers.AsNoTracking()
                                              .FirstOrDefaultAsync(c => c.Id == customerId);
            return customer is null ? null : _mapper.Map<CustomerDto>(customer);
        }

        public async Task<CustomerDto?> AddCustomerAsync(AddCustomerDto dto)
        {
            var entity = _mapper.Map<Customer>(dto);

            await _db.Customers.AddAsync(entity);
            await _db.SaveChangesAsync();

      
            return _mapper.Map<CustomerDto>(entity);
        }

        public async Task<CustomerDto?> UpdateCustomerAsync(Guid customerId, UpdateCustomerDto dto)
        {
            var entity = await _db.Customers.FirstOrDefaultAsync(c => c.Id == customerId);
            if (entity is null) return null;

            _mapper.Map(dto, entity); 
            await _db.SaveChangesAsync();

            return _mapper.Map<CustomerDto>(entity);
        }

        public async Task<bool> DeleteCustomerAsync(Guid customerId)
        {
            var entity = await _db.Customers.FirstOrDefaultAsync(c => c.Id == customerId);
            if (entity is null) return false;

            _db.Customers.Remove(entity);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
