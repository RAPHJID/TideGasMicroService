using System.Net.Http.Json;
using OrderService.Models.DTOs;

namespace OrderService.Services.HttpClients
{
    public class CustomerApiClient : ICustomerApiClient
    {
        private readonly HttpClient _httpClient;

        public CustomerApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<CustomerDto?> GetCustomerByIdAsync(Guid customerId)
        {
            var response = await _httpClient.GetAsync($"/api/customers/{customerId}");
            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadFromJsonAsync<CustomerDto>();
        }
    }
}
