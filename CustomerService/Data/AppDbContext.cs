using CustomerService.Models;
using Microsoft.EntityFrameworkCore;

namespace CustomerService.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Customer> Customers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Customer>(b =>
            {
                b.ToTable("Customer", "dbo");   
                b.HasKey(x => x.Id);
                b.Property(x => x.Id)
                 .HasColumnType("uniqueidentifier")
                 .HasDefaultValueSql("NEWID()");
            });
        }
    }
}
