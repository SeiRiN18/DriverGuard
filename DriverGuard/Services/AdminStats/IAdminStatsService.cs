using DriverGuard.DTO.Admin;

namespace DriverGuard.Services.AdminStats
{
    public interface IAdminStatsService
    {
        Task<AdminStatsDto> GetStatsAsync();
        Task HandleOfflineDevicesAsync();
        Task<IEnumerable<Guid>> GetOfflineDevicesAsync(int minutes);
    }

}
