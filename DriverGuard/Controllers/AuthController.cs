using DriverGuard.DTO.Auth;
using DriverGuard.Models;
using DriverGuard.Services.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DriverGuard.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IJwtService _jwtService;

        public AuthController(IUserService userService, IJwtService jwtService)
        {
            _userService = userService;
            _jwtService = jwtService;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            var existing = await _userService.GetByEmailAsync(dto.Email);
            if (existing != null)
                return BadRequest("Користувач вже існує");

            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = dto.Email,
                PasswordHash = dto.Password,
                Role = UserRole.User,
                CreatedAt = DateTime.UtcNow
            };

            await _userService.CreateAsync(user);

            return Ok();
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var user = await _userService.GetByEmailAsync(dto.Email);
            if (user == null)
                return Unauthorized("Невірний email або пароль");

            bool passwordValid;
            bool needsRehash = false;

            if (user.PasswordHash.StartsWith("$2"))
            {
                passwordValid = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);
            }
            else
            {
                // plain text stored by old buggy register — allow login and rehash
                passwordValid = user.PasswordHash == dto.Password;
                needsRehash = passwordValid;
            }

            if (!passwordValid)
                return Unauthorized("Невірний email або пароль");

            if (needsRehash)
            {
                user.PasswordHash = dto.Password;
                await _userService.UpdateAsync(user);
            }

            var token = _jwtService.GenerateToken(user);

            return Ok(new
            {
                token,
                role = user.Role.ToString()
            });
        }

        [AllowAnonymous]
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto dto)
        {
            var user = await _userService.GetByEmailAsync(dto.Email);
            if (user == null)
                return NotFound("Користувача з таким email не знайдено");

            user.PasswordHash = dto.NewPassword;
            await _userService.UpdateAsync(user);

            return Ok();
        }
    }

}
