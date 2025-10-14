using AutoMapper;
using OrderService.Models;
using OrderService.Models.DTOs;

namespace OrderService.Profiles
{
    public class OrderProfile : Profile
    {
        public OrderProfile()
        {
            CreateMap<Order, OrderReadDTO>();
            CreateMap<OrderCreateDTO, Order>();
        }
    }
}
