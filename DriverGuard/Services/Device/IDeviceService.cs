using DriverGuard.Backend.Models;

namespace DriverGuard.Backend.Services
{
    public interface IDeviceService
    {
        Task<(Device device, string apiKey)> CreateAsync(Device device);
        Task<Device?> GetByIdAsync(Guid id);
        Task<IEnumerable<Device>> GetByUserIdAsync(Guid userId);
        Task<IEnumerable<Device>> GetAllAsync();
        Task UpdateAsync(Device device);
        Task DeleteAsync(Guid id);
    }
}
