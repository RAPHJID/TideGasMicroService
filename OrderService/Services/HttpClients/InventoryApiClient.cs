using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace OrdersService.Services.HttpClients
{
    public interface IInventoryApiClient
    {
        Task<bool> CheckStockAsync(Guid cylinderId, int quantity);
    }

    public class InventoryApiClient : IInventoryApiClient
    {
        private readonly HttpClient _httpClient;

        public InventoryApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<bool> CheckStockAsync(Guid cylinderId, int quantity)
        {
            // Example: GET https://localhost:7037/api/inventory/check/{cylinderId}?quantity=5
            var response = await _httpClient.GetAsync($"api/inventory/check/{cylinderId}?quantity={quantity}");

            if (!response.IsSuccessStatusCode)
                return false;

            var result = await response.Content.ReadFromJsonAsync<bool>();
            return result;
        }
    }
}

