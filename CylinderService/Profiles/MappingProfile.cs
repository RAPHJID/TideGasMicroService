using AutoMapper;
using CylinderService.Models;
using CylinderService.Models.DTOs;

namespace CylinderService.Profiles
{
    public class MappingProfile : Profile
    {

        public MappingProfile()
        {
            // For creating or updating cylinders
            CreateMap<AddUpdateCylinderDto, Cylinder>().ReverseMap();

            // For returning cylinder data
            CreateMap<Cylinder, CylinderDto>().ReverseMap();
        }
    }
}
