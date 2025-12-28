using System.ComponentModel.DataAnnotations;

namespace DriverGuard.DTO.User
{
    public class UserUpdateDto
    {
        [Required(ErrorMessage = "Email є обовʼязковим")]
        [EmailAddress(ErrorMessage = "Невірний формат email")]
        [MaxLength(100)]
        public string Email { get; set; } = null!;

        [MinLength(6, ErrorMessage = "Пароль повинен містити щонайменше 6 символів")]
        [MaxLength(64)]
        public string Password { get; set; } = null!;
    }
}
