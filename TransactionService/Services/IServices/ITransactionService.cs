using TransactionService.Models.DTOs;

namespace TransactionService.Services.IServices
{
    public interface ITransactionService
    {
        Task<IEnumerable<TransactionResponseDTO>> GetAllAsync();
        Task<TransactionResponseDTO?> GetByIdAsync(Guid id);
        Task<TransactionResponseDTO> CreateAsync(CreateUpdateTransactionDTO dto);
        Task<TransactionResponseDTO?> UpdateAsync(Guid id, CreateUpdateTransactionDTO dto);
        Task<bool> DeleteAsync(Guid id);
    }
}
