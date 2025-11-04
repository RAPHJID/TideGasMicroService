using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System;
using OrderService.Models.DTOs;

namespace OrderService.Clients
{
    public class CustomerApiClient
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
