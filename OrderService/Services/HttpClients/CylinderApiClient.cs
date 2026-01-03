using OrderService.Models.DTOs;
using OrderService.Services.IServices;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace OrderService.Services.HttpClients
{
    public class CylinderApiClient : ICylinderApiClient
    {
        private readonly HttpClient _http;

        public CylinderApiClient(HttpClient http)
        {
            _http = http;
        }

        public async Task<CylinderDto?> GetCylinderByIdAsync(Guid id)
        {
            var response = await _http.GetAsync($"api/Cylinder/{id}");

            if (!response.IsSuccessStatusCode)
                return null;

            var body = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<CylinderDto>(
                body,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
    }
}
