using AutoMapper;
using InventoryService.Data;
using InventoryService.Models;
using InventoryService.Models.DTOs;
using InventoryService.Services.IService;
using Microsoft.EntityFrameworkCore;

namespace InventoryService.Services
{
    public class InventorysService : InventoryInterface
    {
        private readonly AppDbContext _appDbContext;
        private readonly IMapper _mapper;

        public InventorysService(AppDbContext appDb, IMapper mapper)
        {
            _appDbContext = appDb;
            _mapper = mapper;
        }


        public async Task<InventoryDto> AddInventoryAsync(AddUpdateInventory inventoryDto)
        {
            var inventory = _mapper.Map<Inventory>(inventoryDto);
            await _appDbContext.AddAsync(inventory);
            await _appDbContext.SaveChangesAsync();
            return _mapper.Map<InventoryDto>(inventory);
        }


        public async Task<bool> DeletedInventoryAsync(Guid inventoryId)
        {
            var inventory = await _appDbContext.Inventorys.FindAsync(inventoryId);
            if (inventory == null) return false;
            _appDbContext.Remove(inventory);
            await _appDbContext.SaveChangesAsync(); 
            return true;
        }

        public async Task<List<InventoryDto>> GetAllInventoriesAsync()
        {
            var inventories = await _appDbContext.Inventorys.ToListAsync();
            return _mapper.Map<List<InventoryDto>>(inventories);
        }

        public async Task<InventoryDto> GetInventoryByIdAsync(Guid inventoryId)
        {
            var inventory = await _appDbContext.Inventorys.FirstOrDefaultAsync(i => i.Id == inventoryId);
            if (inventory == null) return null;
            return _mapper.Map<InventoryDto>(inventory);
        }

        public async Task<InventoryDto> UpdateInventoryAsync(AddUpdateInventory updatedInventory, Guid inventoryId)
        {
            var existing = await _appDbContext.Inventorys.FindAsync(inventoryId);
            if (existing == null) return null;
            _mapper.Map(updatedInventory, existing);
            await _appDbContext.SaveChangesAsync();
            return _mapper.Map<InventoryDto>(existing);

        }
    }
}
