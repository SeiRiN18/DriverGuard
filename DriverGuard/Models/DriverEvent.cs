namespace DriverGuard.Backend.Models
{
    public class DriverEvent
    {
        public Guid Id { get; set; }
        public Guid DeviceId { get; set; }
        public Device? Device { get; set; } = null!;
        public string EventType { get; set; } = null!;
        public int Severity { get; set; }
        public double Confidence   { get; set; }
        public DateTime OccurredAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
