using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using TransactionService.Data;
using TransactionService.Models;
using TransactionService.Models.DTOs;
using TransactionService.Services.IServices;
using System.Text.Json.Serialization;

namespace TransactionService.Services
{
    public class TransactionsService : ITransactionService
    {
        private readonly AppDbContext _context;
        private readonly IHttpClientFactory _httpClientFactory;

        public TransactionsService(AppDbContext context, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
        }

        // ---------------------- Helpers ----------------------

        private async Task<string?> GetCustomerNameAsync(Guid customerId)
        {
            var client = _httpClientFactory.CreateClient("CustomerService");
            var response = await client.GetAsync($"/api/Customer/{customerId}");

            if (!response.IsSuccessStatusCode) return null;

            var json = await response.Content.ReadAsStringAsync();
            var customer = JsonSerializer.Deserialize<CustomerDto>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return customer?.Name;
        }

        private async Task<string?> GetCylinderNameAsync(Guid cylinderId)
        {
            var client = _httpClientFactory.CreateClient("CylinderService");
            var response = await client.GetAsync($"/api/Cylinder/{cylinderId}");

            if (!response.IsSuccessStatusCode) return null;

            var json = await response.Content.ReadAsStringAsync();
            var cylinder = JsonSerializer.Deserialize<CylinderDto>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return cylinder?.Name;
        }

        private async Task<TransactionResponseDTO> MapToDtoAsync(Transaction entity)
        {
            return new TransactionResponseDTO
            {
                Id = entity.Id,
                CustomerId = entity.CustomerId,
                CylinderId = entity.CylinderId,
                Amount = entity.Amount,
                CustomerName = await GetCustomerNameAsync(entity.CustomerId),
                CylinderName = await GetCylinderNameAsync(entity.CylinderId)
            };
        }

        // ---------------------- CRUD ----------------------

        public async Task<IEnumerable<TransactionResponseDTO>> GetAllAsync()
        {
            var transactions = await _context.Transactions.ToListAsync();
            var result = new List<TransactionResponseDTO>();

            foreach (var t in transactions)
            {
                result.Add(await MapToDtoAsync(t));
            }

            return result;
        }

        public async Task<TransactionResponseDTO?> GetByIdAsync(Guid id)
        {
            var entity = await _context.Transactions.FindAsync(id);
            if (entity == null) return null;

            return await MapToDtoAsync(entity);
        }

        public async Task<TransactionResponseDTO> CreateAsync(CreateUpdateTransactionDTO dto)
        {
            var entity = new Transaction
            {
                Id = Guid.NewGuid(),
                CustomerId = dto.CustomerId,
                CylinderId = dto.CylinderId,
                Amount = dto.Amount
            };

            _context.Transactions.Add(entity);
            await _context.SaveChangesAsync();

            return await MapToDtoAsync(entity);
        }

        public async Task<TransactionResponseDTO?> UpdateAsync(Guid id, CreateUpdateTransactionDTO dto)
        {
            var entity = await _context.Transactions.FindAsync(id);
            if (entity == null) return null;

            entity.CustomerId = dto.CustomerId;
            entity.CylinderId = dto.CylinderId;
            entity.Amount = dto.Amount;

            _context.Transactions.Update(entity);
            await _context.SaveChangesAsync();

            return await MapToDtoAsync(entity);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var entity = await _context.Transactions.FindAsync(id);
            if (entity == null) return false;

            _context.Transactions.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }
    }

    // ---------------------- Small DTOs for HttpClient ----------------------

    public class CustomerDto
    {
        public Guid Id { get; set; }

        [JsonPropertyName("fullName")]
        public string Name { get; set; } = string.Empty;
    }

    public class CylinderDto
    {
        public Guid Id { get; set; }

        [JsonPropertyName("brand")]
        public string Name { get; set; } = string.Empty;
    }
}
