using DriverGuard.Backend.DTO.Admin;

namespace DriverGuard.Backend.Services.AdminStats
{
    public interface IAdminStatsService
    {
        Task<AdminStatsDto> GetStatsAsync();
        Task HandleOfflineDevicesAsync();
        Task<IEnumerable<Guid>> GetOfflineDevicesAsync(int minutes);
    }

}
