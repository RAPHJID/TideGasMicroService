using AutoMapper;
using CustomerService.Models;
using CustomerService.Models.DTOs;

namespace CustomerService.Profiles
{
    public class MappingProfile : Profile
    {

        public MappingProfile()
        {
            CreateMap<AddCustomerDto, Customer>();
            CreateMap<UpdateCustomerDto, Customer>();
            CreateMap<Customer, CustomerDto>();
        }
    }
}
