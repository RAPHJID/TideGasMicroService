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

        return inventories.Select(inv => new InventoryDto
        {
            Id = inv.Id,
            CylinderName = dict.TryGetValue(inv.CylinderId, out var c) ? BuildCylinderName(c) : "Unknown",
            QuantityAvailable = inv.QuantityAvailable,
            LastUpdated = inv.LastUpdated
        }).ToList();
    }

    public async Task<InventoryDto?> GetInventoryByIdAsync(Guid inventoryId)
    {
        var inv = await _appDbContext.Inventorys.AsNoTracking().FirstOrDefaultAsync(i => i.Id == inventoryId);
        if (inv == null) return null;

        var cyl = await _cylClient.GetByIdAsync(inv.CylinderId);
        return new InventoryDto
        {
            Id = inv.Id,
            CylinderName = BuildCylinderName(cyl),
            QuantityAvailable = inv.QuantityAvailable,
            LastUpdated = inv.LastUpdated
        };
    }

    public async Task<InventoryDto?> AddInventoryAsync(AddUpdateInventory inventoryDto)
    {
        var cyl = await _cylClient.GetByIdAsync(inventoryDto.CylinderId);
        if (cyl == null) return null;

        var entity = new Inventory
        {
            CylinderId = inventoryDto.CylinderId,
            QuantityAvailable = inventoryDto.QuantityAvailable,
            LastUpdated = DateTime.UtcNow
        };

        _appDbContext.Inventorys.Add(entity);
        await _appDbContext.SaveChangesAsync();

        return new InventoryDto
        {
            Id = entity.Id,
            CylinderName = BuildCylinderName(cyl),
            QuantityAvailable = entity.QuantityAvailable,
            LastUpdated = entity.LastUpdated
        };
    }

    public async Task<InventoryDto?> UpdateInventoryAsync(AddUpdateInventory updatedInventory, Guid inventoryId)
    {
        var existing = await _appDbContext.Inventorys.FindAsync(inventoryId);
        if (existing == null) return null;

        if (existing.CylinderId != updatedInventory.CylinderId)
        {
            var cylCheck = await _cylClient.GetByIdAsync(updatedInventory.CylinderId);
            if (cylCheck == null) return null;
        }

        existing.CylinderId = updatedInventory.CylinderId;
        existing.QuantityAvailable = updatedInventory.QuantityAvailable;
        existing.LastUpdated = DateTime.UtcNow;

        await _appDbContext.SaveChangesAsync();

        var cyl = await _cylClient.GetByIdAsync(existing.CylinderId);
        return new InventoryDto
        {
            Id = existing.Id,
            CylinderName = BuildCylinderName(cyl),
            QuantityAvailable = existing.QuantityAvailable,
            LastUpdated = existing.LastUpdated
        };
    }

    public async Task<bool> DeletedInventoryAsync(Guid inventoryId)
    {
        var inventory = await _appDbContext.Inventorys.FindAsync(inventoryId);
        if (inventory == null) return false;

        _appDbContext.Inventorys.Remove(inventory);
        await _appDbContext.SaveChangesAsync();
        return true;
    }

    // ===== CYLINDER PROXY METHODS =====
    public Task<IEnumerable<CylinderDto>> GetCylindersAsync() => _cylClient.GetAllAsync();
    public Task<CylinderDto?> GetCylinderByIdAsync(Guid id) => _cylClient.GetByIdAsync(id);
    public Task<CylinderDto> CreateCylinderAsync(AddUpdateCylinderDto dto) => _cylClient.CreateAsync(dto);
    public Task<CylinderDto?> UpdateCylinderAsync(Guid id, AddUpdateCylinderDto dto) => _cylClient.UpdateAsync(id, dto);
    public Task<bool> DeleteCylinderAsync(Guid id) => _cylClient.DeleteAsync(id);
}
