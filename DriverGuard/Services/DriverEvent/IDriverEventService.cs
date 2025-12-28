using DriverGuard.Backend.Models;

namespace DriverGuard.Backend.Services
{
    public interface IDriverEventService
    {
        Task CreateAsync(DriverEvent driverEvent);
        Task<IEnumerable<DriverEvent>> GetByDeviceIdAsync(Guid deviceId);
        Task<IEnumerable<DriverEvent>> GetAllAsync();
    }
}
