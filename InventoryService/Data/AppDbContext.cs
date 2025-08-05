using InventoryService.Models;
using Microsoft.EntityFrameworkCore;

namespace InventoryService.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }
        public DbSet<Inventory> Inventorys { get; set; }
    }
}
