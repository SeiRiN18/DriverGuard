using DriverGuard.Backend.Data;
using DriverGuard.Backend.Exceptions;
using DriverGuard.Backend.Models;
using DriverGuard.Backend.Services.Users;
using Microsoft.EntityFrameworkCore;

namespace DriverGuard.Backend.Services
{
    public class UserService : IUserService
    {
        private readonly DriverGuardDbContext _context;

        public UserService(DriverGuardDbContext context)
        {
            _context = context;
        }

        public async Task<User> CreateAsync(User user)
        {
            if (string.IsNullOrWhiteSpace(user.Email))
                throw new ValidationException("Email є обовʼязковим");

            if (string.IsNullOrWhiteSpace(user.PasswordHash))
                throw new ValidationException("Пароль є обовʼязковим");

            var exists = await _context.Users
                .AnyAsync(u => u.Email == user.Email);

            if (exists)
                throw new ValidationException(
                    "Користувач з таким email вже існує");

            user.Id = Guid.NewGuid();
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);
            user.CreatedAt = DateTime.UtcNow;

            if (user.Role == default)
                user.Role = UserRole.User;

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return user;
        }

        public async Task<User?> GetByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new ValidationException("Id користувача є обовʼязковим");

            var user = await _context.Users.FindAsync(id);

            if (user == null)
                throw new NotFoundException("Користувача не знайдено");

            return user;
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ValidationException("Email є обовʼязковим");

            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email);

        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task UpdateAsync(User user)
        {
            if (user.Id == Guid.Empty)
                throw new ValidationException("Id користувача є обовʼязковим");

            var existing = await _context.Users.FindAsync(user.Id);

            if (existing == null)
                throw new NotFoundException("Користувача не знайдено");

            if (!string.IsNullOrWhiteSpace(user.Email))
                existing.Email = user.Email;

            if (!string.IsNullOrWhiteSpace(user.PasswordHash))
                existing.PasswordHash =
                    BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);

            if (user.Role != default)
                existing.Role = user.Role;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new ValidationException("Id користувача є обовʼязковим");

            var user = await _context.Users.FindAsync(id);

            if (user == null)
                throw new NotFoundException("Користувача не знайдено");

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }
    }
}
