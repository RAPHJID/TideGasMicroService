using InventoryService.Data;
using InventoryService.Models;
using InventoryService.Models.DTOs;
using InventoryService.Services.IService;
using Microsoft.EntityFrameworkCore;

public class InventorysService : InventoryInterface
{
    private readonly AppDbContext _context;

    public InventorysService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<InventoryDto>> GetAllInventoriesAsync()
    {
        return await _context.Inventorys
            .Select(i => new InventoryDto
            {
                cylinderId = i.CylinderId,
                QuantityAvailable = i.QuantityAvailable
            })
            .ToListAsync();
    }

    public async Task<InventoryDto?> GetInventoryByIdAsync(Guid cylinderId)
    {
        var item = await _context.Inventorys.FindAsync(cylinderId);
        if (item == null) return null;

        return new InventoryDto
        {
            cylinderId = item.CylinderId,
            QuantityAvailable = item.QuantityAvailable
        };
    }

    public async Task AddInventoryAsync(AddUpdateInventory dto)
    {
        var newItem = new Inventory
        {
            CylinderId = dto.CylinderId,
            QuantityAvailable = dto.QuantityAvailable
        };

        _context.Inventorys.Add(newItem);
        await _context.SaveChangesAsync();
    }

    public async Task IncreaseQuantityAsync(Guid cylinderId, int quantity)
    {
        var item = await _context.Inventorys.FindAsync(cylinderId);
        if (item == null) return;

        item.QuantityAvailable += quantity;
        await _context.SaveChangesAsync();
    }

    public async Task DecreaseQuantityAsync(Guid cylinderId, int quantity)
    {
        var item = await _context.Inventorys.FindAsync(cylinderId);
        if (item == null)
            throw new Exception("Cylinder not found in inventory.");

        if (item.QuantityAvailable < quantity)
            throw new Exception("Not enough stock to decrease.");

        item.QuantityAvailable -= quantity;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteInventoryAsync(Guid cylinderId)
    {
        var item = await _context.Inventorys.FindAsync(cylinderId);
        if (item == null) return;

        _context.Inventorys.Remove(item);
        await _context.SaveChangesAsync();
    }

   
    public async Task<bool> CheckStockAsync(Guid cylinderId, int quantity)
    {
        var item = await _context.Inventorys.FindAsync(cylinderId);
        if (item == null) return false;

        return item.QuantityAvailable >= quantity;
    }
}
