using System.ComponentModel.DataAnnotations;

namespace DriverGuard.DTO.Auth
{
    public class LoginDto
    {
        [Required(ErrorMessage = "Email є обов'язковим")]
        [EmailAddress(ErrorMessage = "Некоректний формат email")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Пароль є обов'язковим")]
        public string Password { get; set; } = null!;
    }

}
