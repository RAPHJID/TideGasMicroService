using Microsoft.EntityFrameworkCore;
using TransactionService.Data;
using TransactionService.Models;
using TransactionService.Models.DTOs;
using TransactionService.Services.IServices;
using AutoMapper;

namespace TransactionService.Services
{
    public class TransactionsService : ITransactionService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public TransactionsService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<TransactionResponseDTO>> GetAllAsync()
        {
            var transactions = await _context.Transactions.ToListAsync();
            return _mapper.Map<IEnumerable<TransactionResponseDTO>>(transactions);
        }

        public async Task<TransactionResponseDTO?> GetByIdAsync(Guid id)
        {
            var transaction = await _context.Transactions.FindAsync(id);
            return transaction == null ? null : _mapper.Map<TransactionResponseDTO>(transaction);
        }

        public async Task<TransactionResponseDTO> CreateAsync(CreateUpdateTransactionDTO dto)
        {
            var entity = _mapper.Map<Transaction>(dto);
            entity.Id = Guid.NewGuid();
            entity.Date = DateTime.UtcNow;

            _context.Transactions.Add(entity);
            await _context.SaveChangesAsync();

            return _mapper.Map<TransactionResponseDTO>(entity);
        }

        public async Task<TransactionResponseDTO?> UpdateAsync(Guid id, CreateUpdateTransactionDTO dto)
        {
            var transaction = await _context.Transactions.FindAsync(id);
            if (transaction == null) return null;

            transaction.CustomerId = dto.CustomerId;
            transaction.CylinderId = dto.CylinderId;
            transaction.Amount = dto.Amount;

            await _context.SaveChangesAsync();

            return _mapper.Map<TransactionResponseDTO>(transaction);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var transaction = await _context.Transactions.FindAsync(id);
            if (transaction == null) return false;

            _context.Transactions.Remove(transaction);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
