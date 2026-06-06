using System.ComponentModel.DataAnnotations;

namespace DriverGuard.DTO.User
{
    public class UpdateFcmTokenDto
    {
        [Required]
        public string FcmToken { get; set; } = null!;
    }
}
