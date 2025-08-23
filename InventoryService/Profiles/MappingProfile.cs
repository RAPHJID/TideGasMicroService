using AutoMapper;
using InventoryService.Models;
using InventoryService.Models.DTOs;

namespace InventoryService.Profiles
{
    public class MappingProfile : Profile
    {

        public MappingProfile()
        {
            // For creating, updating inventory & also returning inventory data
            CreateMap<InventoryDto, Inventory>().ReverseMap();
            CreateMap<AddUpdateInventory, Inventory>().ReverseMap();
        }
    }
}
