using DriverGuard.Backend.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace DriverGuard.Backend.Data
{
    public class DriverGuardDbContextFactory
        : IDesignTimeDbContextFactory<DriverGuardDbContext>
    {
        public DriverGuardDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<DriverGuardDbContext>();

            optionsBuilder.UseNpgsql(
                "Host=localhost;Port=5432;Database=driverguard_db;Username=postgres;Password=olegsemo18"
            );

            return new DriverGuardDbContext(optionsBuilder.Options);
        }
    }
}
