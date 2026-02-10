using InventoryService.Common;
using InventoryService.Data;
using InventoryService.Models;
using InventoryService.Models.DTOs;
using InventoryService.Services.HttpClients;
using InventoryService.Services.IService;
using Microsoft.EntityFrameworkCore;

public class InventorysService : IInventoryService
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

    // ================= READ =================

    public async Task<IEnumerable<InventoryDto>> GetAllInventoriesAsync()
    {
        var inventories = await _context.Inventorys.ToListAsync();
        var result = new List<InventoryDto>();

        foreach (var inv in inventories)
        {
            var cylinderResult = await _cylinderClient.GetByIdAsync(inv.CylinderId);

            var dto = new InventoryDto
            {
                CylinderId = inv.CylinderId,
                QuantityAvailable = inv.QuantityAvailable
            };

            if (cylinderResult.IsSuccess && cylinderResult.Value != null)
            {
                var cylinder = cylinderResult.Value;
                dto.Size = cylinder.Size ?? "N/A";
                dto.Brand = cylinder.Brand;
                dto.Status = cylinder.Status;
                dto.Condition = cylinder.Condition;
            }

            result.Add(dto);
        }

        return result;
    }

    public async Task<Result<InventoryDto>> GetInventoryByCylinderIdAsync(Guid cylinderId)
    {
        var inventory = await _context.Inventorys
            .FirstOrDefaultAsync(i => i.CylinderId == cylinderId);

        if (inventory == null)
            return Result<InventoryDto>.Failure("Inventory not found.");

        var cylinderResult = await _cylinderClient.GetByIdAsync(cylinderId);

        if (!cylinderResult.IsSuccess || cylinderResult.Value == null)
            return Result<InventoryDto>.Failure("Failed to fetch cylinder details.");

        var cylinder = cylinderResult.Value;

        var dto = new InventoryDto
        {
            CylinderId = inventory.CylinderId,
            QuantityAvailable = inventory.QuantityAvailable,
            Size = cylinder.Size ?? "N/A",
            Brand = cylinder.Brand,
            Status = cylinder.Status,
            Condition = cylinder.Condition
        };

        return Result<InventoryDto>.Success(dto);
    }

    // ================= WRITE =================

    public async Task<Result<bool>> CreateInventoryAsync(Guid cylinderId, decimal initialQuantity)
    {
        var exists = await _context.Inventorys
            .AnyAsync(i => i.CylinderId == cylinderId);

        if (exists)
            return Result<bool>.Failure("Inventory already exists for this cylinder.");

        var inventory = new Inventory
        {
            CylinderId = cylinderId,
            QuantityAvailable = initialQuantity,
            LastUpdated = DateTime.UtcNow
        };

        _context.Inventorys.Add(inventory);
        await _context.SaveChangesAsync();

        return Result<bool>.Success(true);
    }

    public async Task<Result<InventoryDto>> AdjustInventoryAsync(
        Guid cylinderId,
        decimal quantityChange,
        string updatedByUserId)
    {
        var inventory = await _context.Inventorys
            .FirstOrDefaultAsync(i => i.CylinderId == cylinderId);

        if (inventory == null)
            return Result<InventoryDto>.Failure("Inventory not found.");

        if (inventory.QuantityAvailable + quantityChange < 0)
            return Result<InventoryDto>.Failure("Insufficient stock.");

        inventory.QuantityAvailable += quantityChange;
        inventory.LastUpdated = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return await GetInventoryByCylinderIdAsync(cylinderId);
    }

    // ================= VALIDATION =================

    public async Task<Result<bool>> CheckStockAsync(Guid cylinderId, decimal requiredQuantity)
    {
        var inventory = await _context.Inventorys
            .FirstOrDefaultAsync(i => i.CylinderId == cylinderId);

        if (inventory == null)
            return Result<bool>.Failure("Inventory not found.");

        return Result<bool>.Success(inventory.QuantityAvailable >= requiredQuantity);
    }

    // ================= ADMIN =================

    public async Task<Result<bool>> DeleteInventoryAsync(Guid cylinderId)
    {
        var inventory = await _context.Inventorys
            .FirstOrDefaultAsync(i => i.CylinderId == cylinderId);

        if (inventory == null)
            return Result<bool>.Failure("Inventory not found.");

        _context.Inventorys.Remove(inventory);
        await _context.SaveChangesAsync();

        return Result<bool>.Success(true);
    }
}
