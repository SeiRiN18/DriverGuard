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
            return Ok(await _deviceService.GetAllAsync());
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
