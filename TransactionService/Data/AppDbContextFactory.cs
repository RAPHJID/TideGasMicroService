using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace TransactionService.Data
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

            optionsBuilder.UseSqlServer(
                "Server=localhost\\SQLEXPRESS;Database=TideTransactionDb;Trusted_Connection=True;Encrypt=False;");

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}
