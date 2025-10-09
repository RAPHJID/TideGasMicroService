
using AutoMapper;
using TransactionService.Models;
using TransactionService.Models.DTOs;

namespace TransactionService.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Transaction, TransactionResponseDTO>();
            CreateMap<CreateUpdateTransactionDTO, Transaction>();
        }
    }
}

