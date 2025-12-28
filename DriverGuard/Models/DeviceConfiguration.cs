namespace DriverGuard.Backend.Models
{
    public class DeviceConfiguration
    {
        public Guid DeviceId { get; set; }
        public Device? Device { get; set; } = null!;
        public double DrowsinessThreshold { get; set; }
        public double AttentionThreshold { get; set; }
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
