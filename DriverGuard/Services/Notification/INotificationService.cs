using DriverGuard.Backend.Models;

namespace DriverGuard.Backend.Services
{
    public interface INotificationService
    {
        // Створюється автоматично при критичній події
        Task<Notification> CreateAsync(Notification notification);

        // ADMIN: сповіщення по пристрою
        Task<IEnumerable<Notification>> GetByDeviceIdAsync(Guid deviceId);

        // USER: мої сповіщення
        Task<IEnumerable<Notification>> GetByUserIdAsync(Guid userId);

        // USER: позначити як прочитане
        Task<bool> MarkAsReadAsync(Guid notificationId, Guid userId);
    }
}
