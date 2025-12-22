using OrderService.Models.DTOs;
using System.Net.Http.Json;
using System.Text.Json;
using static System.Net.WebRequestMethods;

namespace OrderService.Services.HttpClients
{
    public class CustomerApiClient : ICustomerApiClient
    {
        private readonly HttpClient _httpClient;

        public CustomerApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<CustomerDto?> GetCustomerByIdAsync(Guid id)
        {
            var response = await _httpClient.GetAsync($"api/Customer/{id}");

            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadFromJsonAsync<CustomerDto>(
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
        }


    }
}
