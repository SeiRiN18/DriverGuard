using DriverGuard.Backend.Data;
using DriverGuard.Backend.DTO.DeviceConfiguration;
using DriverGuard.Backend.Models;
using DriverGuard.Backend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DriverGuard.Backend.Controllers
{
    [ApiController]
    [Route("api/devices/{deviceId}/configuration")]
    public class DeviceConfigurationsController : ControllerBase
    {
        private readonly IDeviceConfigurationService _service;

        public DeviceConfigurationsController(IDeviceConfigurationService service)
        {
            _service = service;
        }

        // READ
        [HttpGet]
        public async Task<ActionResult<DeviceConfigurationReadDto>> Get(Guid deviceId)
        {
            var config = await _service.GetByDeviceIdAsync(deviceId);
            if (config == null) return NotFound();

            return new DeviceConfigurationReadDto
            {
                DeviceId = config.DeviceId,
                DrowsinessThreshold = config.DrowsinessThreshold,
                AttentionThreshold = config.AttentionThreshold,
                UpdatedAt = config.UpdatedAt
            };
        }

        // CREATE OR UPDATE
        [HttpPut]
        public async Task<IActionResult> Update(Guid deviceId, DeviceConfigurationUpdateDto dto)
        {
            var config = new DeviceConfiguration
            {
                DeviceId = deviceId,
                DrowsinessThreshold = dto.DrowsinessThreshold,
                AttentionThreshold = dto.AttentionThreshold,
                UpdatedAt = DateTime.UtcNow
            };

            await _service.CreateOrUpdateAsync(config);
            return NoContent();
        }
    }
}