using DriverGuard.Data;
using DriverGuard.Exceptions;
using DriverGuard.Models;
using DriverGuard.Services.Fcm;
using Microsoft.EntityFrameworkCore;

namespace DriverGuard.Services
{
    public class NotificationService : INotificationService
    {
        private readonly DriverGuardDbContext _context;
        private readonly IFcmService _fcmService;

        public NotificationService(DriverGuardDbContext context, IFcmService fcmService)
        {
            _context = context;
            _fcmService = fcmService;
        }

        // =====================================================
        // CREATE NOTIFICATION (BUSINESS LOGIC)
        // Викликається автоматично з DriverEventService
        // =====================================================
        public async Task<Notification> CreateAsync(Notification notification)
        {
            // ==========================
            // VALIDATION
            // ==========================
            if (notification == null)
                throw new ValidationException("Сповіщення не може бути порожнім");

            if (notification.UserId == Guid.Empty)
                throw new ValidationException("UserId є обовʼязковим");

            if (notification.DeviceId == Guid.Empty)
                throw new ValidationException("DeviceId є обовʼязковим");

            if (notification.DriverEventId == Guid.Empty)
                throw new ValidationException("DriverEventId є обовʼязковим");

            if (string.IsNullOrWhiteSpace(notification.Type))
                throw new ValidationException("Тип сповіщення є обовʼязковим");

            if (string.IsNullOrWhiteSpace(notification.Message))
                throw new ValidationException("Текст сповіщення є обовʼязковим");

            // ==========================
            // CREATE
            // ==========================
            notification.Id = Guid.NewGuid();
            notification.CreatedAt = DateTime.UtcNow;
            notification.IsRead = false;

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            // Push notification via FCM
            var user = await _context.Users.FindAsync(notification.UserId);
            if (user?.FcmToken != null)
            {
                var title = notification.Type == "CRITICAL" ? "Критичний стан водія!" : "Попередження";
                await _fcmService.SendAsync(
                    user.FcmToken,
                    title,
                    notification.Message,
                    new Dictionary<string, string> { ["notificationId"] = notification.Id.ToString() }
                );
            }

            return notification;
        }

        // =====================================================
        // ADMIN: GET BY DEVICE
        // =====================================================
        public async Task<IEnumerable<Notification>> GetByDeviceIdAsync(Guid deviceId)
        {
            if (deviceId == Guid.Empty)
                throw new ValidationException("DeviceId не може бути порожнім");

            return await _context.Notifications
                .Where(n => n.DeviceId == deviceId)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        // =====================================================
        // USER: GET MY NOTIFICATIONS
        // =====================================================
        public async Task<IEnumerable<Notification>> GetByUserIdAsync(Guid userId)
        {
            if (userId == Guid.Empty)
                throw new ValidationException("UserId не може бути порожнім");

            return await _context.Notifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        // =====================================================
        // USER: MARK AS READ
        // =====================================================
        public async Task<bool> MarkAsReadAsync(Guid notificationId, Guid userId)
        {
            if (notificationId == Guid.Empty)
                throw new ValidationException("NotificationId не може бути порожнім");

            if (userId == Guid.Empty)
                throw new ValidationException("UserId не може бути порожнім");

            var notification = await _context.Notifications
                .FirstOrDefaultAsync(n => n.Id == notificationId);

            if (notification == null)
                throw new NotFoundException("Сповіщення не знайдено");

            if (notification.UserId != userId)
                throw new ForbiddenException(
                    "Ви не маєте доступу до цього сповіщення");

            if (notification.IsRead)
                return true; // ідемпотентність

            notification.IsRead = true;
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
