using DriverGuard.Models;

namespace DriverGuard.Services
{
    public interface IDeviceConfigurationService
    {
        Task<DeviceConfiguration> CreateOrUpdateAsync(DeviceConfiguration config);
        Task<DeviceConfiguration?> GetByDeviceIdAsync(Guid deviceId);
    }
}
