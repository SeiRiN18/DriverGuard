using System;
using System.ComponentModel.DataAnnotations;

namespace DriverGuard.Backend.DTO.DriverEvent
{
    public class DriverEventCreateDto
    {
        public Guid DeviceId { get; set; }

        [Required(ErrorMessage = "Тип події є обовʼязковим")]
        [MaxLength(50, ErrorMessage = "Тип події не може перевищувати 50 символів")]
        public string EventType { get; set; } = null!;

        [Range(1, 5, ErrorMessage = "Рівень небезпеки повинен бути в діапазоні 1–5")]
        public int Severity { get; set; }

        [Range(0.0, 1.0, ErrorMessage = "Confidence повинен бути в діапазоні 0.0–1.0")]
        public double Confidence { get; set; }

        [Required(ErrorMessage = "Час виникнення події є обовʼязковим")]
        public DateTime OccurredAt { get; set; }
    }
}
