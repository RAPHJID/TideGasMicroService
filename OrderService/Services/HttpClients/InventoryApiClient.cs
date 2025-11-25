using System.Net.Http.Json;
using OrderService.Models.DTOs;
using OrderService.Services.IServices;


namespace OrderService.Services.HttpClients
{
    public class InventoryApiClient : IInventoryApiClient
    {
        private readonly HttpClient _http;

        public InventoryApiClient(HttpClient http)
        {
            _http = http;
        }

        public async Task<CylinderDto?> GetCylinderByIdAsync(Guid cylinderId)
        {
            try
            {
                return await _http.GetFromJsonAsync<CylinderDto>($"api/cylinders/{cylinderId}");
            }
            catch
            {
                return null; // prevents crashing OrderService
            }
        }

        public async Task<bool> CheckStockAsync(Guid cylinderId, int quantity)
        {
            try
            {
                var response = await _http.GetFromJsonAsync<bool>(
                    $"api/cylinders/{cylinderId}/check-stock?quantity={quantity}"
                );

                return response;
            }
            catch
            {
                return false; // fail-safe
            }
        }
    }
}
