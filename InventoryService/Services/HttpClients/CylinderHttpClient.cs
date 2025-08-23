using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using InventoryService.Models.DTOs;

namespace InventoryService.Services.HttpClients
{
    public class CylinderHttpClient : ICylinderHttpClient
    {
        private readonly HttpClient _client;

        public CylinderHttpClient(HttpClient client)
        {
            _client = client;
        }

        public async Task<IEnumerable<CylinderDto>> GetAllAsync()
        {
            var resp = await _client.GetAsync("api/Cylinder/all");
            resp.EnsureSuccessStatusCode();
            return await resp.Content.ReadFromJsonAsync<IEnumerable<CylinderDto>>() ?? Array.Empty<CylinderDto>();
        }

        public async Task<CylinderDto?> GetByIdAsync(Guid id)
        {
            var resp = await _client.GetAsync($"api/Cylinder/{id}");
            if (resp.StatusCode == HttpStatusCode.NotFound) return null;
            resp.EnsureSuccessStatusCode();
            return await resp.Content.ReadFromJsonAsync<CylinderDto>();
        }

        public async Task<CylinderDto> CreateAsync(AddUpdateCylinderDto dto)
        {
            var resp = await _client.PostAsJsonAsync("api/Cylinder", dto);
            resp.EnsureSuccessStatusCode();
            return await resp.Content.ReadFromJsonAsync<CylinderDto>();
        }

        public async Task<CylinderDto?> UpdateAsync(Guid id, AddUpdateCylinderDto dto)
        {
            var resp = await _client.PutAsJsonAsync($"api/Cylinder/{id}", dto);
            if (resp.StatusCode == HttpStatusCode.NotFound) return null;
            resp.EnsureSuccessStatusCode();
            return await resp.Content.ReadFromJsonAsync<CylinderDto>();
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var resp = await _client.DeleteAsync($"api/Cylinder/{id}");
            if (resp.StatusCode == HttpStatusCode.NotFound) return false;
            resp.EnsureSuccessStatusCode();
            return true;
        }
    }
}
