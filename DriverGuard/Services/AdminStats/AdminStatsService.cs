using DriverGuard.Data;
using DriverGuard.DTO.Admin;
using DriverGuard.Models;
using DriverGuard.Services.AdminStats;
using Microsoft.EntityFrameworkCore;

namespace DriverGuard.Services
{
    public class AdminStatsService : IAdminStatsService
    {
        private readonly DriverGuardDbContext _context;

        public AdminStatsService(DriverGuardDbContext context)
        {
            _context = context;
        }

        public async Task<AdminStatsDto> GetStatsAsync()
        {
            return new AdminStatsDto
            {
                Users = await _context.Users.CountAsync(),
                Devices = await _context.Devices.CountAsync(),
                Events = await _context.DriverEvents.CountAsync(),
                CriticalEvents = await _context.DriverEvents
                    .CountAsync(e => e.Severity >= 4),
                Notifications = await _context.Notifications.CountAsync(),
                UnreadNotifications = await _context.Notifications
                    .CountAsync(n => !n.IsRead)
            };
        }
        public async Task<IEnumerable<Guid>> GetOfflineDevicesAsync(int minutes = 5)
        {
            var threshold = DateTime.UtcNow.AddMinutes(-minutes);

            return await _context.Devices
                .Where(d => d.IsActive && d.LastSeenAt < threshold)
                .Select(d => d.Id)
                .ToListAsync();
        }
        public async Task HandleOfflineDevicesAsync()
        {
            var threshold = DateTime.UtcNow.AddMinutes(-5);

            var devices = await _context.Devices
                .Where(d => d.IsActive && d.LastSeenAt < threshold)
                .ToListAsync();

            foreach (var device in devices)
                device.IsActive = false;

            await _context.SaveChangesAsync();
        }


    }
}
