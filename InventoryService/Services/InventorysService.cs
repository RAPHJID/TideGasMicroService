using AutoMapper;
using InventoryService.Data;
using InventoryService.Models;
using InventoryService.Models.DTOs;
using InventoryService.Services.HttpClients;
using InventoryService.Services.IService;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// Generic service result wrapper
public record ServiceResult<T>(bool Success, string? Error, T? Data);

public class InventorysService : InventoryInterface
{
    private readonly AppDbContext _appDbContext;
    private readonly IMapper _mapper;
    private readonly ICylinderHttpClient _cylClient;

    public InventorysService(AppDbContext appDb, IMapper mapper, ICylinderHttpClient cylClient)
    {
        _appDbContext = appDb;
        _mapper = mapper;
        _cylClient = cylClient;
    }

    // Helper to build cylinder name
    private static string BuildCylinderName(CylinderDto? cyl)
    {
        if (cyl == null) return "Unknown";
        return !string.IsNullOrWhiteSpace(cyl.Brand) && !string.IsNullOrWhiteSpace(cyl.Size)
            ? $"{cyl.Brand} {cyl.Size}"
            : cyl.Brand ?? cyl.Size ?? cyl.Id.ToString();
    }

    // ===== INVENTORY METHODS =====

    public async Task<List<InventoryDto>> GetAllInventoriesAsync()
    {
        var inventories = await _appDbContext.Inventorys.AsNoTracking().ToListAsync();

        var distinctIds = inventories.Select(i => i.CylinderId).Distinct().ToList();
        var cylinderTasks = distinctIds.Select(id => _cylClient.GetByIdAsync(id));
        var cylinderResults = await Task.WhenAll(cylinderTasks);

        var dict = distinctIds.Select((id, i) => new { id, cyl = cylinderResults[i] })
                              .ToDictionary(x => x.id, x => x.cyl);

        return inventories.Select(inv =>
        {
            var dto = _mapper.Map<InventoryDto>(inv);
            dto.CylinderName = dict.TryGetValue(inv.CylinderId, out var c) ? BuildCylinderName(c) : "Unknown";
            return dto;
        }).ToList();
    }

    public async Task<ServiceResult<InventoryDto>> GetInventoryByIdAsync(Guid inventoryId)
    {
        var inv = await _appDbContext.Inventorys.AsNoTracking().FirstOrDefaultAsync(i => i.Id == inventoryId);
        if (inv == null)
            return new(false, "Inventory not found", null);

        var cyl = await _cylClient.GetByIdAsync(inv.CylinderId);
        var dto = _mapper.Map<InventoryDto>(inv);
        dto.CylinderName = BuildCylinderName(cyl);

        return new(true, null, dto);
    }

    public async Task<ServiceResult<InventoryDto>> AddInventoryAsync(AddUpdateInventory inventoryDto)
    {
        var cyl = await _cylClient.GetByIdAsync(inventoryDto.CylinderId);
        if (cyl == null)
            return new(false, "Cylinder not found", null);

        var entity = _mapper.Map<Inventory>(inventoryDto);
        entity.LastUpdated = DateTime.UtcNow;

        _appDbContext.Inventorys.Add(entity);
        await _appDbContext.SaveChangesAsync();

        var dto = _mapper.Map<InventoryDto>(entity);
        dto.CylinderName = BuildCylinderName(cyl);

        return new(true, null, dto);
    }

    public async Task<ServiceResult<InventoryDto>> UpdateInventoryAsync(AddUpdateInventory updatedInventory, Guid inventoryId)
    {
        var existing = await _appDbContext.Inventorys.FindAsync(inventoryId);
        if (existing == null)
            return new(false, "Inventory not found", null);

        if (existing.CylinderId != updatedInventory.CylinderId)
        {
            var cylCheck = await _cylClient.GetByIdAsync(updatedInventory.CylinderId);
            if (cylCheck == null)
                return new(false, "New cylinder not found", null);
        }

        existing.CylinderId = updatedInventory.CylinderId;
        existing.QuantityAvailable = updatedInventory.QuantityAvailable;
        existing.LastUpdated = DateTime.UtcNow;

        await _appDbContext.SaveChangesAsync();

        var cyl = await _cylClient.GetByIdAsync(existing.CylinderId);
        var dto = _mapper.Map<InventoryDto>(existing);
        dto.CylinderName = BuildCylinderName(cyl);

        return new(true, null, dto);
    }
    public async Task<bool> CheckStockAsync(Guid cylinderId, int quantity)
    {
        var item = await _appDbContext.Inventorys.FindAsync(cylinderId);

        if (item == null)
            return false;

        return item.QuantityAvailable >= quantity;
    }


    public async Task DecreaseQuantityAsync(Guid cylinderId, int quantity)
    {
        var item = await _appDbContext.Inventorys.FirstOrDefaultAsync(i => i.CylinderId == cylinderId);
        if (item == null)
            throw new Exception("Inventory item not found.");

        if (item.QuantityAvailable < quantity)
            throw new Exception("Not enough stock to decrease.");

        item.QuantityAvailable -= quantity;
        item.LastUpdated = DateTime.UtcNow;

        await _appDbContext.SaveChangesAsync();
    }

    public async Task IncreaseQuantityAsync(Guid cylinderId, int quantity)
    {
        var item = await _appDbContext.Inventorys.FirstOrDefaultAsync(i => i.CylinderId == cylinderId);
        if (item == null)
            throw new Exception("Inventory not found.");

        item.QuantityAvailable += quantity;
        item.LastUpdated = DateTime.UtcNow;

        await _appDbContext.SaveChangesAsync();
    }

    public async Task<ServiceResult<bool>> DeletedInventoryAsync(Guid inventoryId)
    {
        var inventory = await _appDbContext.Inventorys.FindAsync(inventoryId);
        if (inventory == null)
            return new(false, "Inventory not found", false);

        _appDbContext.Inventorys.Remove(inventory);
        await _appDbContext.SaveChangesAsync();

        return new(true, null, true);
    }

    // ===== CYLINDER PROXY METHODS =====
    public Task<IEnumerable<CylinderDto>> GetCylindersAsync() => _cylClient.GetAllAsync();
    public Task<CylinderDto?> GetCylinderByIdAsync(Guid id) => _cylClient.GetByIdAsync(id);
    public Task<CylinderDto> CreateCylinderAsync(AddUpdateCylinderDto dto) => _cylClient.CreateAsync(dto);
    public Task<CylinderDto?> UpdateCylinderAsync(Guid id, AddUpdateCylinderDto dto) => _cylClient.UpdateAsync(id, dto);
    public Task<bool> DeleteCylinderAsync(Guid id) => _cylClient.DeleteAsync(id);
}
