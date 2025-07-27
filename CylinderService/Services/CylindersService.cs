using AutoMapper;
using CylinderService.Data;
using CylinderService.Models;
using CylinderService.Models.DTOs;
using CylinderService.Services.IServices;
using Microsoft.EntityFrameworkCore;

namespace CylinderService.Services
{
    public class CylindersService : ICylinder
    {
        private readonly CylinderDbContext _context;
        private readonly IMapper _mapper;

        public CylindersService(CylinderDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        
        public async Task<IEnumerable<CylinderDto>> GetAllCylindersAsync()
        {
            var cylinders = await _context.Cylinders.ToListAsync();
            return _mapper.Map<IEnumerable<CylinderDto>>(cylinders);
        }

        public async Task<CylinderDto?> GetCylinderByIdAsync(Guid id)
        {
            var cylinder = await _context.Cylinders.FirstOrDefaultAsync(c => c.Id == id);
            if (cylinder == null) return null;
            return _mapper.Map<CylinderDto>(cylinder);

        }

        public async Task<CylinderDto> AddCylinderAsync(AddUpdateCylinderDto cylinderDto)
        {
            var cylinder = _mapper.Map<Cylinder>(cylinderDto);
            await _context.Cylinders.AddAsync(cylinder);
            await _context.SaveChangesAsync();
            return _mapper.Map<CylinderDto>(cylinder);
        }

        public async Task<CylinderDto> UpdateCylinderAsync(Guid id, AddUpdateCylinderDto cylinderDto)
        {
            var cylinder = await _context.Cylinders.FindAsync(id);
            _mapper.Map(cylinderDto, cylinder);
            await _context.SaveChangesAsync();
            return _mapper.Map<CylinderDto>(cylinder);
        }

        public async Task<bool> DeleteCylinderAsync(Guid id)

        {
            var cylinder = await _context.Cylinders.FindAsync(id);
            if (cylinder == null) return false;
            _context.Cylinders.Remove(cylinder);
            await _context.SaveChangesAsync();
            return true;
        }



    }
}
