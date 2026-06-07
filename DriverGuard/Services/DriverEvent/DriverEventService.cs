using DriverGuard.Data;
using DriverGuard.Exceptions;
using DriverGuard.Models;
using Microsoft.EntityFrameworkCore;

namespace DriverGuard.Services
{
    public class DriverEventService : IDriverEventService
    {
        private readonly DriverGuardDbContext _context;
        private readonly INotificationService _notificationService;

        public DriverEventService(
            DriverGuardDbContext context,
            INotificationService notificationService)
        {
            _context = context;
            _notificationService = notificationService;
        }

        // =====================================================
        // CREATE DRIVER EVENT (BUSINESS LOGIC CORE)
        // =====================================================
        public async Task CreateAsync(DriverEvent driverEvent)
        {
            if (driverEvent == null)
                throw new ValidationException("Подія не може бути порожньою");

            if (driverEvent.DeviceId == Guid.Empty)
                throw new ValidationException("DeviceId є обовʼязковим");

            if (string.IsNullOrWhiteSpace(driverEvent.EventType))
                throw new ValidationException("EventType є обовʼязковим");

            if (driverEvent.Severity < 0 || driverEvent.Severity > 5)
                throw new ValidationException(
                    "Severity повинно бути в межах 0..5");

            if (driverEvent.Confidence < 0 || driverEvent.Confidence > 1)
                throw new ValidationException(
                    "Confidence повинно бути в межах 0..1");

            var device = await _context.Devices
                .Include(d => d.DeviceConfiguration)
                .FirstOrDefaultAsync(d => d.Id == driverEvent.DeviceId);

            if (device == null)
                throw new NotFoundException("Пристрій не знайдено");

            driverEvent.Id = Guid.NewGuid();
            driverEvent.CreatedAt = DateTime.UtcNow;
            device.LastSeenAt = DateTime.UtcNow;
            device.IsActive = true;

            _context.DriverEvents.Add(driverEvent);
            await _context.SaveChangesAsync();


            var config = device.DeviceConfiguration;

            bool isCritical =
                driverEvent.Severity >= 4 &&
                config != null &&
                driverEvent.Confidence >= config.DrowsinessThreshold;

            if (isCritical)
            {
                await _notificationService.CreateAsync(new Notification
                {
                    UserId = device.UserId,
                    DeviceId = device.Id,
                    DriverEventId = driverEvent.Id,
                    Type = "CRITICAL",
                    Message = $"Критичний стан водія (confidence: {driverEvent.Confidence:F2})"
                });
            }

        }

        // =====================================================
        // READ EVENTS BY DEVICE
        // =====================================================
        public async Task<IEnumerable<DriverEvent>> GetByDeviceIdAsync(Guid deviceId)
        {
            if (deviceId == Guid.Empty)
                throw new ValidationException("DeviceId не може бути порожнім");

            return await _context.DriverEvents
                .Where(e => e.DeviceId == deviceId)
                .OrderByDescending(e => e.OccurredAt)
                .ToListAsync();
        }

        // =====================================================
        // READ ALL EVENTS (ADMIN)
        // =====================================================
        public async Task<IEnumerable<DriverEvent>> GetAllAsync()
        {
            return await _context.DriverEvents
                .OrderByDescending(e => e.OccurredAt)
                .ToListAsync();
        }
    }
}
