using System.Net.Http.Json;
using OrderService.Models.DTOs;
using OrderService.Services.IServices;
using InventoryService.Common;


namespace OrderService.Services.HttpClients
{
    public class InventoryApiClient : IInventoryApiClient
    {
        private readonly HttpClient _http;

        public InventoryApiClient(HttpClient http)
        {
            _http = http;
            // TEMP DEBUG LINE
            Console.WriteLine($"[DEBUG] InventoryApiClient BaseAddress = {_http.BaseAddress}");
        }

        public async Task<Result<bool>> CheckStockAsync(Guid cylinderId, int quantity)
        {
            var response = await _http.GetAsync(
                $"api/Inventory/{cylinderId}/check-stock?quantity={quantity}");

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                return Result<bool>.Failure(error);
            }
        }

        public async Task<Result<bool>> DecreaseStockAsync(Guid cylinderId, int quantity)
        {
            var response = await _http.PatchAsync(
                $"api/Inventory/{cylinderId}/decrease/{quantity}",
                null);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                return Result<bool>.Failure(error);
            }

            return Result<bool>.Success(true);
        }

    }
}
