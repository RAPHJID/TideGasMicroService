using InventoryService.Data;
using InventoryService.Models;
using InventoryService.Models.DTOs;
using InventoryService.Services.IService;
using InventoryService.Services.HttpClients;
using Microsoft.EntityFrameworkCore;

public class InventorysService : InventoryInterface
{
    private readonly AppDbContext _context;
    private readonly ICylinderHttpClient _cylinderClient;

    public InventorysService(
        AppDbContext context,
        ICylinderHttpClient cylinderClient)
    {
        _context = context;
        _cylinderClient = cylinderClient;
    }

    public async Task<IEnumerable<InventoryDto>> GetAllInventoriesAsync()
    {
        var inventories = await _context.Inventorys.ToListAsync();

        var result = new List<InventoryDto>();

        foreach (var i in inventories)
        {
            // Call CylinderService to get size/name
            var cylinder = await _cylinderClient.GetByIdAsync(i.CylinderId);

            result.Add(new InventoryDto
            {
                cylinderId = i.CylinderId,
                QuantityAvailable = i.QuantityAvailable,
                Name = cylinder?.Name ?? "Unknown",
                Size = cylinder?.Size ?? "N/A"
            });
        }

        return result;
    }

    public async Task<InventoryDto?> GetInventoryByIdAsync(Guid cylinderId)
    {
        var item = await _context.Inventorys.FindAsync(cylinderId);
        if (item == null) return null;

        var cylinder = await _cylinderClient.GetByIdAsync(cylinderId);

        return new InventoryDto
        {
            cylinderId = item.CylinderId,
            QuantityAvailable = item.QuantityAvailable,
            Name = cylinder?.Name ?? "Unknown",
            Size = cylinder?.Size ?? "N/A"
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

    // ✅ This is used by OrderService
    public async Task<bool> CheckStockAsync(Guid cylinderId, int quantity)
    {
        var item = await _context.Inventorys.FindAsync(cylinderId);
        if (item == null) return false;

        return item.QuantityAvailable >= quantity;
    }
}
