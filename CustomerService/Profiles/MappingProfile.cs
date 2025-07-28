using AutoMapper;
using CustomerService.Models;
using CustomerService.Models.DTOs;

namespace CustomerService.Profiles
{
    public class MappingProfile : Profile
    {

        public MappingProfile()
        {
            // For creating, updating customer & also returning customer data
            CreateMap<CustomerDto, Customer>().ReverseMap();
        }
    }
}
