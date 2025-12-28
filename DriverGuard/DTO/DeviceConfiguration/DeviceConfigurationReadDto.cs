namespace DriverGuard.DTO.DeviceConfiguration
{
    public class DeviceConfigurationReadDto
    {
        public Guid DeviceId { get; set; }
        public double DrowsinessThreshold { get; set; }
        public double AttentionThreshold { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

}
