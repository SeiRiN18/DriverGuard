namespace DriverGuard.Backend.DTO.Device
{
    public class DeviceReadDto
    {
        public Guid Id { get; set; }
        public string SerialNumber { get; set; } = null!;
        public bool IsActive { get; set; }
        public DateTime? LastSeenAt { get; set; }
    }

}
