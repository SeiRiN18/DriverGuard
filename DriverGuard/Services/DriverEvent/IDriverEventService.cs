using DriverGuard.Models;

namespace DriverGuard.Services
{
    public interface IDriverEventService
    {
        Task CreateAsync(DriverEvent driverEvent);
        Task<IEnumerable<DriverEvent>> GetByDeviceIdAsync(Guid deviceId);
        Task<IEnumerable<DriverEvent>> GetAllAsync();
    }
}
