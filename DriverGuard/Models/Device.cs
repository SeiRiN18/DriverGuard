namespace DriverGuard.Models
{
    public class Device
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public User? User { get; set; }
        public string SerialNumber { get; set; } = null!;
        public string ApiKeyHash { get; set; } = null!;
        public bool IsActive { get; set; }
        public DateTime? LastSeenAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DeviceConfiguration? DeviceConfiguration { get; set; }

    }
}
