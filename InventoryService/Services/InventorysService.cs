using InventoryService.Data;
using InventoryService.Models;
using InventoryService.Models.DTOs;
using InventoryService.Services.IService;
using InventoryService.Services.HttpClients;
using Microsoft.EntityFrameworkCore;
using InventoryService.Common;

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




    public async Task<Result<InventoryDto>> GetInventoryByIdAsync(Guid cylinderId)
    {
        var item = await _context.Inventorys
            .FirstOrDefaultAsync(i => i.CylinderId == cylinderId);

        if (item == null)
            return Result<InventoryDto>.Failure("Inventory item not found.");

        var cylinderResult = await _cylinderClient.GetByIdAsync(cylinderId);

        if (!cylinderResult.IsSuccess)
            return Result<InventoryDto>.Failure(cylinderResult.Error!);

        var cylinder = cylinderResult.Value!;

        return Result<InventoryDto>.Success(new InventoryDto
        {
            CylinderId = item.CylinderId,
            QuantityAvailable = item.QuantityAvailable,
            Size = cylinder.Size ?? "N/A",
            Brand = cylinder.Brand,
            Status = cylinder.Status,
            Condition = cylinder.Condition
        });
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


    public async Task<Result<bool>> IncreaseQuantityAsync(Guid cylinderId, int quantity)
    {
        if (quantity <= 0)
            return Result<bool>.Failure("Quantity must be greater than zero.");

        var item = await _context.Inventorys
            .FirstOrDefaultAsync(i => i.CylinderId == cylinderId);

        if (item == null)
            return Result<bool>.Failure("Inventory item not found.");

        item.QuantityAvailable += quantity;
        item.LastUpdated = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Result<bool>.Success(true);
    }


    public async Task<Result<bool>> DecreaseQuantityAsync(Guid cylinderId, int quantity)
    {
        if (quantity <= 0)
            return Result<bool>.Failure("Quantity must be greater than zero.");

        var item = await _context.Inventorys
            .FirstOrDefaultAsync(i => i.CylinderId == cylinderId);

        if (item == null)
            return Result<bool>.Failure("Inventory item not found.");

        if (item.QuantityAvailable < quantity)
            return Result<bool>.Failure("Not enough stock available.");

        item.QuantityAvailable -= quantity;
        item.LastUpdated = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Result<bool>.Success(true);
    }



    public async Task DeleteInventoryAsync(Guid cylinderId)
    {
        var item = await _context.Inventorys.FindAsync(cylinderId);
        if (item == null) return;

        _context.Inventorys.Remove(item);
        await _context.SaveChangesAsync();
    }

    public async Task<Result<bool>> CheckStockAsync(Guid cylinderId, int quantity)
    {
        if (quantity <= 0)
            return Result<bool>.Failure("Quantity must be greater than zero.");

        var item = await _context.Inventorys
            .FirstOrDefaultAsync(i => i.CylinderId == cylinderId);

        if (item == null)
            return Result<bool>.Failure("Inventory item not found.");

        if (item.QuantityAvailable < quantity)
            return Result<bool>.Failure("Insufficient stock.");

        return Result<bool>.Success(true);
    }

}
