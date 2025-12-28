using DriverGuard.Models;
using Microsoft.EntityFrameworkCore;

namespace DriverGuard.Data
{
    public class DriverGuardDbContext : DbContext
    {
        public DriverGuardDbContext(DbContextOptions<DriverGuardDbContext> options) 
          : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<Device> Devices => Set<Device>();
        public DbSet<DriverEvent> DriverEvents => Set<DriverEvent>();
        public DbSet<DeviceConfiguration> DeviceConfigurations => Set<DeviceConfiguration>();
        public DbSet<Notification> Notifications => Set<Notification>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DeviceConfiguration>()
                .HasKey(dc => dc.DeviceId);

            modelBuilder.Entity<Notification>()
                .HasOne(n => n.Device)
                .WithMany()
                .HasForeignKey(n => n.DeviceId);

            modelBuilder.Entity<Notification>()
                .HasOne(n => n.DriverEvent)
                .WithMany()
                .HasForeignKey(n => n.DriverEventId);

            base.OnModelCreating(modelBuilder);
        }



    }
}
