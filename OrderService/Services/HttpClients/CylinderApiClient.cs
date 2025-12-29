using System.Net;
using System.Net.Http.Json;
using OrderService.Models.DTOs;
using OrderService.Services.IServices;

namespace OrderService.Services.HttpClients
{
    public class CylinderApiClient : ICylinderApiClient
    {
        private readonly HttpClient _http;

        public CylinderApiClient(HttpClient http)
        {
            _http = http;
        }

        public async Task<CylinderDto?> GetByIdAsync(Guid cylinderId)
        {
            var response = await _http.GetAsync($"/api/Cylinder/{cylinderId}");

            if (response.StatusCode == HttpStatusCode.NotFound)
                return null;

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<CylinderDto>();
        }
    }
}
