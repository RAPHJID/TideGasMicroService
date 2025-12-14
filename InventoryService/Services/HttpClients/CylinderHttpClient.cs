using InventoryService.Common;
using InventoryService.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace InventoryService.Services.HttpClients
{
    public class CylinderHttpClient : ICylinderHttpClient
    {
        private readonly HttpClient _client;

        public CylinderHttpClient(HttpClient client)
        {
            _client = client;
        }

        // central helper to read body and throw a custom exception
        private static async Task HandleNonSuccessAsync(HttpResponseMessage resp)
        {
            var body = resp.Content == null ? string.Empty : await resp.Content.ReadAsStringAsync();
            throw new CylinderApiException(resp.StatusCode, body);
        }

        public async Task<IEnumerable<CylinderDto>> GetAllAsync()
        {
            var resp = await _client.GetAsync("api/Cylinder/all");

            if (!resp.IsSuccessStatusCode)
                await HandleNonSuccessAsync(resp);

            return await resp.Content.ReadFromJsonAsync<IEnumerable<CylinderDto>>()
                   ?? Array.Empty<CylinderDto>();
        }

        public async Task<Result<CylinderDto>> GetByIdAsync(Guid id)
        {
            var resp = await _client.GetAsync($"api/Cylinder/{id}");

            if (resp.StatusCode == HttpStatusCode.NotFound)
                return Result<CylinderDto>.Failure("Cylinder not found");

            if (!resp.IsSuccessStatusCode)
            {
                var body = await resp.Content.ReadAsStringAsync();
                return Result<CylinderDto>.Failure(
                    $"Cylinder API error {(int)resp.StatusCode}: {body}"
                );
            }

            var cylinder = await resp.Content.ReadFromJsonAsync<CylinderDto>();

            if (cylinder == null)
                return Result<CylinderDto>.Failure("Empty response from CylinderService");

            return Result<CylinderDto>.Success(cylinder);
        }



        public async Task<CylinderDto> CreateAsync(AddUpdateCylinderDto dto)
        {
            var resp = await _client.PostAsJsonAsync("api/Cylinders", dto);
            if (!resp.IsSuccessStatusCode)
                await HandleNonSuccessAsync(resp);
            return await resp.Content.ReadFromJsonAsync<CylinderDto>();
        }

        public async Task<CylinderDto?> UpdateAsync(Guid id, AddUpdateCylinderDto dto)
        {
            var resp = await _client.PutAsJsonAsync($"api/Cylinders/{id}", dto);
            if (resp.StatusCode == HttpStatusCode.NotFound) return null;
            if (!resp.IsSuccessStatusCode)
                await HandleNonSuccessAsync(resp);
            return await resp.Content.ReadFromJsonAsync<CylinderDto>();
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var resp = await _client.DeleteAsync($"api/Cylinders/{id}");
            if (resp.StatusCode == HttpStatusCode.NotFound) return false;
            if (!resp.IsSuccessStatusCode)
                await HandleNonSuccessAsync(resp);
            return true;
        }
    }
}
