using DriverGuard.Models;

namespace DriverGuard.Services.Users
{
    public interface IUserService
    {
        // CREATE
        Task<User> CreateAsync(User user);

        // READ
        Task<User?> GetByIdAsync(Guid id);
        Task<User?> GetByEmailAsync(string email);
        Task<IEnumerable<User>> GetAllAsync();

        // UPDATE
        Task UpdateAsync(User user);

        // DELETE
        Task DeleteAsync(Guid id);
    }
}
