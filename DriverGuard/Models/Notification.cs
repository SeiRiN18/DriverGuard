using System;

namespace DriverGuard.Models
{
    public class Notification
    {
        public Guid Id { get; set; }

        // =========================
        // Ownership
        // =========================
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;

        public Guid DeviceId { get; set; }
        public Device Device { get; set; } = null!;

        // =========================
        // Source event
        // =========================
        public Guid DriverEventId { get; set; }
        public DriverEvent DriverEvent { get; set; } = null!;

        // =========================
        // Notification data
        // =========================
        public string Type { get; set; } = null!; // WARNING, CRITICAL
        public string Message { get; set; } = null!;

        public bool IsRead { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
