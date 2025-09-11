using AutoMapper;
using TransactionService.Models;
using TransactionService.Models.DTOs;

namespace TransactionService.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CreateUpdateTransactionDTO, Transaction>()
                .ForMember(dest => dest.TransactionDate, opt => opt.MapFrom(_ => DateTime.UtcNow));

            CreateMap<Transaction, TransactionResponseDTO>();
        }
    }
}
