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

        public async Task<Result<TransactionResponseDTO>> CreateTransactionAsync(CreateUpdateTransactionDTO dto)
        {
            var response = await _http.PostAsJsonAsync("api/Transaction", dto);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                return Result<TransactionResponseDTO>.Failure(error);
            }

            var created = await response.Content.ReadFromJsonAsync<TransactionResponseDTO>();

            return Result<TransactionResponseDTO>.Success(created!);
        }










    }
}
