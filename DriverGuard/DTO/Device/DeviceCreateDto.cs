using System.ComponentModel.DataAnnotations;

namespace DriverGuard.DTO.Device
{
    public class DeviceCreateDto
    {
        [Required(ErrorMessage = "UserId є обов'язковим")]
        public Guid UserId { get; set; }

        [Required(ErrorMessage = "Серійний номер є обов'язковим")]
        [StringLength(64, MinimumLength = 4,
            ErrorMessage = "Серійний номер повинен містити від 4 до 64 символів")]
        public string SerialNumber { get; set; } = null!;
    }
}
