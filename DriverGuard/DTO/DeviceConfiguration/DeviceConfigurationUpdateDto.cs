using System.ComponentModel.DataAnnotations;

namespace DriverGuard.Backend.DTO.DeviceConfiguration
{
    public class DeviceConfigurationUpdateDto
    {
        [Range(0.0, 1.0, ErrorMessage = "Поріг сонливості повинен бути в діапазоні 0.0–1.0")]
        public double DrowsinessThreshold { get; set; }

        [Range(0.0, 1.0, ErrorMessage = "Поріг уваги повинен бути в діапазоні 0.0–1.0")]
        public double AttentionThreshold { get; set; }
    }
}
