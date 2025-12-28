namespace DriverGuard.DTO.DriverEvent
{
    public class DriverEventReadDto
    {
        public Guid Id { get; set; }
        public Guid DeviceId { get; set; }
        public string EventType { get; set; } = null!;
        public int Severity { get; set; }
        public double Confidence { get; set; }
        public DateTime OccurredAt { get; set; }
    }

}
