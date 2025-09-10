using Microsoft.EntityFrameworkCore;
using TransactionService.Models;

namespace TransactionService.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

    
        public DbSet<Transaction> Transactions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // configure defaults or constraints
            modelBuilder.Entity<Transaction>()
                .Property(t => t.TransactionDate)
                .HasDefaultValueSql("GETUTCDATE()");
        }
    }
}
