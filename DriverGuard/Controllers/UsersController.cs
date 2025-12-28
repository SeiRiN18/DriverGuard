using DriverGuard.DTO.User;
using DriverGuard.Models;
using DriverGuard.Services.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[ApiController]
[Route("api/users")]
[Authorize] // 🔒 ВСІ ендпоінти захищені
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    // ===============================
    // READ ALL (ADMIN ONLY)
    // ===============================
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<IEnumerable<UserReadDto>>> GetAll()
    {
        var users = await _userService.GetAllAsync();

        return users.Select(u => new UserReadDto
        {
            Id = u.Id,
            Email = u.Email,
            CreatedAt = u.CreatedAt
        }).ToList();
    }

    // ===============================
    // READ BY ID (ADMIN або ВЛАСНИК)
    // ===============================
    [HttpGet("{id}")]
    public async Task<ActionResult<UserReadDto>> GetById(Guid id)
    {
        var currentUserId = Guid.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!
        );

        var isAdmin = User.IsInRole("Admin");

        if (!isAdmin && currentUserId != id)
            return Forbid();

        var user = await _userService.GetByIdAsync(id);
        if (user == null) return NotFound();

        return new UserReadDto
        {
            Id = user.Id,
            Email = user.Email,
            CreatedAt = user.CreatedAt
        };
    }

    // ===============================
    // UPDATE (ADMIN або ВЛАСНИК)
    // ===============================
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, UserUpdateDto dto)
    {
        var currentUserId = Guid.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!
        );

        var isAdmin = User.IsInRole("Admin");

        if (!isAdmin && currentUserId != id)
            return Forbid();

        var user = await _userService.GetByIdAsync(id);
        if (user == null) return NotFound();

        user.Email = dto.Email;

        // 🔐 ХЕШУЄМО ПАРОЛЬ
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

        await _userService.UpdateAsync(user);
        return NoContent();
    }

    // ===============================
    // DELETE (ADMIN ONLY)
    // ===============================
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _userService.DeleteAsync(id);
        return NoContent();
    }
}
