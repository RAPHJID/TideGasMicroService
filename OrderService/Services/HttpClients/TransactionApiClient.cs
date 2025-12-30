using InventoryService.Common;
using OrderService.Models.DTOs;

namespace OrderService.Services.HttpClients
{
    public class TransactionApiClient : ITransactionApiClient
    {
        private readonly HttpClient _http;

        public TransactionApiClient(HttpClient http)
        {
            _http = http;
        }

        public async Task<Result<TransactionResponseDTO>> CreateTransactionAsync(CreateTransactionDTO dto)
        {
            var response = await _http.PostAsJsonAsync("/api/Transaction", dto);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                return Result<TransactionResponseDTO>.Failure(error);
            }

            var transaction =
                await response.Content.ReadFromJsonAsync<TransactionResponseDTO>();

            if (transaction is null)
                return Result<TransactionResponseDTO>.Failure("Transaction returned null.");

            return Result<TransactionResponseDTO>.Success(transaction);
        }

        







    }
}
