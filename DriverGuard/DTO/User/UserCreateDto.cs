using System.ComponentModel.DataAnnotations;

namespace DriverGuard.DTO.User
{
    public class UserCreateDto
    {
        [Required(ErrorMessage = "Email є обовʼязковим")]
        [EmailAddress(ErrorMessage = "Невірний формат email")]
        [MaxLength(100, ErrorMessage = "Email не може перевищувати 100 символів")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Пароль є обовʼязковим")]
        [MinLength(6, ErrorMessage = "Пароль повинен містити щонайменше 6 символів")]
        [MaxLength(64, ErrorMessage = "Пароль не може перевищувати 64 символи")]
        public string Password { get; set; } = null!;
    }
}
