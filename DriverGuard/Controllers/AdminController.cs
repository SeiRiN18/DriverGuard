using DriverGuard.Services;
using DriverGuard.Services.AdminStats;
using DriverGuard.Services.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DriverGuard.Controllers
{
    [ApiController]
    [Route("api/admin")]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IDeviceService _deviceService;
        private readonly IAdminStatsService _statsService;

        public AdminController(
            IUserService userService,
            IDeviceService deviceService,
            IAdminStatsService statsService)
        {
            _userService = userService;
            _deviceService = deviceService;
            _statsService = statsService;
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers()
        {
            return Ok(await _userService.GetAllAsync());
        }

        [HttpGet("devices")]
        public async Task<IActionResult> GetAllDevices()
        {
            var devices = await _deviceService.GetAllAsync();
            var threshold = DateTime.UtcNow.AddSeconds(-30);

            var result = devices.Select(d => new
            {
                d.Id,
                d.SerialNumber,
                d.UserId,
                IsActive = d.LastSeenAt.HasValue && d.LastSeenAt.Value > threshold,
                d.LastSeenAt
            });

            return Ok(result);
        }
        [HttpPost("devices/check-offline")]
        public async Task<IActionResult> CheckOffline()
        {
            await _statsService.HandleOfflineDevicesAsync();
            return Ok("Offline devices processed");
        }
        [HttpGet("stats")]
        public async Task<IActionResult> GetStats()
        {
            var stats = await _statsService.GetStatsAsync();
            return Ok(stats);
        }

    }

}
