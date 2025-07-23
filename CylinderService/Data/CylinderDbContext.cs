using CylinderService.Models;
using Microsoft.EntityFrameworkCore;

namespace CylinderService.Data
{
    public class CylinderDbContext : DbContext
    {
        public CylinderDbContext(DbContextOptions<CylinderDbContext> options)
            : base(options)
        {
        }
        public DbSet<Cylinder> Cylinders { get; set; } 
       
    }
}
