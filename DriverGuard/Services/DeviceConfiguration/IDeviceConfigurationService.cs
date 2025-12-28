using DriverGuard.Backend.Models;

namespace DriverGuard.Backend.Services
{
    public interface IDeviceConfigurationService
    {
        Task<DeviceConfiguration> CreateOrUpdateAsync(DeviceConfiguration config);
        Task<DeviceConfiguration?> GetByDeviceIdAsync(Guid deviceId);
    }
}
