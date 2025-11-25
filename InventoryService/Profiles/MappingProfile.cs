using AutoMapper;
using InventoryService.Models;
using InventoryService.Models.DTOs;

namespace InventoryService.Profiles
{
    public class MappingProfile : Profile
    {

        public MappingProfile()
        {
            CreateMap<Inventory, InventoryDto>()
           .ForMember(dest => dest.cylinderId, opt => opt.MapFrom(src => src.CylinderId));

            CreateMap<InventoryDto, Inventory>()
                .ForMember(dest => dest.CylinderId, opt => opt.MapFrom(src => src.cylinderId));

            CreateMap<AddUpdateInventory, Inventory>();
            CreateMap<Inventory, AddUpdateInventory>();
        }
    }
}
