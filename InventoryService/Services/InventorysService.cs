using AutoMapper;
using InventoryService.Data;
using InventoryService.Models;
using InventoryService.Models.DTOs;
using InventoryService.Services.HttpClients;
using InventoryService.Services.IService;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InventoryService.Services
{
    public class InventorysService : InventoryInterface
    {
        private readonly AppDbContext _appDbContext;
        private readonly IMapper _mapper;
        private readonly ICylinderHttpClient _cylClient;

        // Added ICylinderHttpClient injection (keeps AppDbContext and IMapper)
        public InventorysService(AppDbContext appDb, IMapper mapper, ICylinderHttpClient cylClient)
        {
            _appDbContext = appDb;
            _mapper = mapper;
            _cylClient = cylClient;
        }

        // ===== INVENTORY METHODS (unchanged) =====
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

        // ===== CYLINDER METHODS (proxy to CylinderService via ICylinderHttpClient) =====
        public Task<IEnumerable<CylinderDto>> GetCylindersAsync()
            => _cylClient.GetAllAsync();

        public Task<CylinderDto?> GetCylinderByIdAsync(Guid id)
            => _cylClient.GetByIdAsync(id);

        public Task<CylinderDto> CreateCylinderAsync(AddUpdateCylinderDto dto)
            => _cylClient.CreateAsync(dto);

        public Task<CylinderDto?> UpdateCylinderAsync(Guid id, AddUpdateCylinderDto dto)
            => _cylClient.UpdateAsync(id, dto);

        public Task<bool> DeleteCylinderAsync(Guid id)
            => _cylClient.DeleteAsync(id);
    }
}
