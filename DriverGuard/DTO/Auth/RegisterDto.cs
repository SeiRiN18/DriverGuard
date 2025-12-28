using System.ComponentModel.DataAnnotations;

namespace DriverGuard.DTO.Auth
{
    public class RegisterDto
    {
        [Required(ErrorMessage = "Email є обов'язковим")]
        [EmailAddress(ErrorMessage = "Некоректний формат email")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Пароль є обов'язковим")]
        [MinLength(6, ErrorMessage = "Пароль повинен містити мінімум 6 символів")]
        public string Password { get; set; } = null!;
    }
}
