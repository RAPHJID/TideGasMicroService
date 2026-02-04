using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using CylinderService.Data;
using CylinderService.Models;
using CylinderService.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace CylinderService.Services
{
    // Note: implement the fully-qualified interface to avoid namespace mix-ups.
    public class CylindersService : CylinderService.Services.IServices.ICylinder
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
            return cylinder is null ? null : _mapper.Map<CylinderDto>(cylinder);
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
            if (cylinder is null)
                return null!; // controller will treat null as NotFound

            _mapper.Map(cylinderDto, cylinder);
            await _context.SaveChangesAsync();
            return _mapper.Map<CylinderDto>(cylinder);
        }

        public async Task<CylinderDto?> UpdateDailySalesAsync(Guid cylinderId,string staffUserId,int quantitySoldToday)
        {
            var cylinder = await _context.Cylinders.FindAsync(cylinderId);
            if (cylinder == null)
                return null;

            cylinder.SoldToday = quantitySoldToday;
            cylinder.LastUpdatedByStaffId = staffUserId;
            cylinder.LastUpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return _mapper.Map<CylinderDto>(cylinder);
        }


        public async Task<bool> DeleteCylinderAsync(Guid id)
        {
            var cylinder = await _context.Cylinders.FindAsync(id);
            if (cylinder is null) return false;

            _context.Cylinders.Remove(cylinder);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
