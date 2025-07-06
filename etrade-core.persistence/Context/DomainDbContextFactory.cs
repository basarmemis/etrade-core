using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace etrade_core.persistence.Context
{
    /// <summary>
    /// DomainDbContext için design-time factory
    /// Migration oluştururken kullanılır
    /// </summary>
    public class DomainDbContextFactory : IDesignTimeDbContextFactory<DomainDbContext>
    {
        public DomainDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<DomainDbContext>();
            
            // Development connection string - migration oluştururken kullanılır
            var connectionString = "Host=localhost;Port=5432;Database=etradecore;Username=postgres;Password=a1";
            
            optionsBuilder.UseNpgsql(connectionString);

            return new DomainDbContext(optionsBuilder.Options);
        }
    }
} 