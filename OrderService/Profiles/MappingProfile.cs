using AutoMapper;
using OrderService.Models;
using OrderService.Models.DTOs;
using OrderService.Models.Enums;

namespace OrderService.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Model → ReadDTO
            CreateMap<Order, OrderReadDTO>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

            // CreateDTO → Model
            CreateMap<OrderCreateDTO, Order>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => MapStatus(src.Status)));

            // UpdateDTO → Model
            CreateMap<OrderUpdateDTO, Order>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => MapStatus(src.Status)));
        }

        private static OrderStatus MapStatus(string status)
        {
            if (string.IsNullOrWhiteSpace(status))
                return OrderStatus.Pending;

            return Enum.TryParse<OrderStatus>(status, true, out var parsed)
                ? parsed
                : OrderStatus.Pending;
        }
    }
}

