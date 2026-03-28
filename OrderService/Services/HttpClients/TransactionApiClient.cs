using InventoryService.Common;
using OrderService.Models.DTOs;
using OrderService.Services.HttpClients;
using System.Text.Json;

public class TransactionApiClient : ITransactionApiClient
{
    private readonly HttpClient _http;

    public TransactionApiClient(HttpClient http)
    {
        _http = http;
    }

    public async Task<Result<TransactionReadDTO>> CreateTransactionAsync(
    CreateUpdateTransactionDTO dto,
    string token
)
    {
        try
        {
           
            _http.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue(
                    "Bearer",
                    token.Replace("Bearer ", "")
                );

            var response = await _http.PostAsJsonAsync("api/Transaction", dto);

            var body = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return Result<TransactionReadDTO>.Failure(
                    $"Transaction API error ({(int)response.StatusCode}): {body}");
            }

            var transaction = JsonSerializer.Deserialize<TransactionReadDTO>(
                body,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return Result<TransactionReadDTO>.Success(transaction!);
        }
        catch (Exception ex)
        {
            return Result<TransactionReadDTO>.Failure(
                $"TransactionService error: {ex.Message}");
        }
    }
}
