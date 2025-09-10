using Microsoft.EntityFrameworkCore;
using TransactionService.Data;
using TransactionService.Models;
using TransactionService.Models.DTOs;
using TransactionService.Services.IServices;

namespace TransactionService.Services
{
    public class TransactionsService : ITransactionService
    {
        private readonly AppDbContext _context;

        public TransactionsService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TransactionResponseDTO>> GetAllAsync()
        {
            var transactions = await _context.Transactions.ToListAsync();

            return transactions.Select(t => new TransactionResponseDTO
            {
                Id = t.TransactionId,
                CustomerName = $"Customer-{t.CustomerId}", 
                CylinderName = $"Cylinder-{t.CylinderId}", 
                TotalPrice = t.Quantity * 100
            });
        }

        public async Task<TransactionResponseDTO?> GetByIdAsync(Guid id)
        {
            var transaction = await _context.Transactions.FindAsync(id);
            if (transaction == null) return null;

            return new TransactionResponseDTO
            {
                Id = transaction.TransactionId,
                CustomerName = $"Customer-{transaction.CustomerId}",
                CylinderName = $"Cylinder-{transaction.CylinderId}",
                TotalPrice = transaction.Quantity * 100
            };
        }

        public async Task<TransactionResponseDTO> CreateAsync(CreateUpdateTransactionDTO dto)
        {
            var transaction = new Transaction
            {
                TransactionId = Guid.NewGuid(),
                CylinderId = dto.CylinderId,
                CustomerId = dto.CustomerId,
                Quantity = dto.Quantity,
                TransactionDate = DateTime.UtcNow
            };

            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();

            return new TransactionResponseDTO
            {
                Id = transaction.TransactionId,
                CustomerName = $"Customer-{transaction.CustomerId}",
                CylinderName = $"Cylinder-{transaction.CylinderId}",
                TotalPrice = transaction.Quantity * 100
            };
        }

        public async Task<TransactionResponseDTO?> UpdateAsync(Guid id, CreateUpdateTransactionDTO dto)
        {
            var transaction = await _context.Transactions.FindAsync(id);
            if (transaction == null) return null;

            transaction.CylinderId = dto.CylinderId;
            transaction.CustomerId = dto.CustomerId;
            transaction.Quantity = dto.Quantity;
            transaction.TransactionDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return new TransactionResponseDTO
            {   
                Id= transaction.TransactionId,
                CustomerName = $"Customer-{transaction.CustomerId}",
                CylinderName = $"Cylinder-{transaction.CylinderId}",
                TotalPrice = transaction.Quantity * 100
            };
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
