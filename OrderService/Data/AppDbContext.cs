using Microsoft.EntityFrameworkCore;
using OrderService.Models;

namespace OrderService.Data
    {
        public class AppDbContext : DbContext
        {
            public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

            public DbSet<Order> Orders { get; set; }

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                //Store enums as strings in the DB
                modelBuilder.Entity<Order>()
                    .Property(o => o.Status)
                    .HasConversion<string>();

                base.OnModelCreating(modelBuilder);
            }
        }
    }



