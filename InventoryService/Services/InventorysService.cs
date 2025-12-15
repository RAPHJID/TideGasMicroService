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

        foreach (var inv in inventories)
        {
            var cylinderResult = await _cylinderClient.GetByIdAsync(inv.CylinderId);

            if (!cylinderResult.IsSuccess || cylinderResult.Value == null)
            {
                // Cylinder service failed or cylinder missing
                result.Add(new InventoryDto
                {
                    CylinderId = inv.CylinderId,
                    QuantityAvailable = inv.QuantityAvailable,
                    Size = "N/A",
                    Brand = "Unknown",
                    Status = "Unknown",
                    Condition = "Unknown"
                });

                continue;
            }

            var cylinder = cylinderResult.Value;

            result.Add(new InventoryDto
            {
                CylinderId = inv.CylinderId,
                QuantityAvailable = inv.QuantityAvailable,
                Size = cylinder.Size ?? "N/A",
                Brand = cylinder.Brand,
                Status = cylinder.Status,
                Condition = cylinder.Condition
            });
        }

        return result;
    }




    public async Task<InventoryDto?> GetInventoryByIdAsync(Guid cylinderId)
    {
        var item = await _context.Inventorys
            .FirstOrDefaultAsync(i => i.CylinderId == cylinderId);

        if (item == null)
            return null;

        var cylinderResult = await _cylinderClient.GetByIdAsync(cylinderId);

        if (!cylinderResult.IsSuccess)
            throw new Exception(cylinderResult.Error);

        return new InventoryDto
        {
            CylinderId = item.CylinderId,
            QuantityAvailable = item.QuantityAvailable,
            Size = cylinderResult.Value!.Size ?? "N/A",
            Brand = cylinderResult.Value.Brand,
            Status = cylinderResult.Value.Status,
            Condition = cylinderResult.Value.Condition
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
