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

        public async Task<Result<bool>> CreateTransactionAsync(CreateTransactionDTO dto)
        {
            var response = await _http.PostAsJsonAsync("/api/Transaction", dto);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                return Result<bool>.Failure(error);
            }

            return Result<bool>.Success(true);
        }

    }
}
