using DriverGuard.DTO.Device;
using DriverGuard.Models;
using DriverGuard.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[ApiController]
[Route("api/devices")]
[Authorize]
public class DevicesController : ControllerBase
{
    private readonly IDeviceService _deviceService;

    public DevicesController(IDeviceService deviceService)
    {
        _deviceService = deviceService;
    }


    [HttpPost]
    public async Task<IActionResult> Create(DeviceCreateDto dto)
    {
        var userId = Guid.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!
        );

        var device = new Device
        {
            UserId = userId,
            SerialNumber = dto.SerialNumber
        };

        var (createdDevice, apiKey) = await _deviceService.CreateAsync(device);

        return CreatedAtAction(
            nameof(GetById),
            new { id = createdDevice.Id },
            new
            {
                createdDevice.Id,
                createdDevice.SerialNumber,
                apiKey
            }
        );

    }

    [HttpGet("my")]
    public async Task<ActionResult<IEnumerable<DeviceReadDto>>> GetMy()
    {
        var userId = Guid.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!
        );

        var devices = await _deviceService.GetByUserIdAsync(userId);

        return devices.Select(d => new DeviceReadDto
        {
            Id = d.Id,
            SerialNumber = d.SerialNumber,
            IsActive = d.IsActive,
            LastSeenAt = d.LastSeenAt
        }).ToList();
    }


    [HttpGet("{id}")]
    public async Task<ActionResult<DeviceReadDto>> GetById(Guid id)
    {
        var device = await _deviceService.GetByIdAsync(id);
        if (device == null) return NotFound();

        var userId = Guid.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!
        );

        var isAdmin = User.IsInRole("Admin");

        if (!isAdmin && device.UserId != userId)
            return Forbid();

        return new DeviceReadDto
        {
            Id = device.Id,
            SerialNumber = device.SerialNumber,
            IsActive = device.IsActive,
            LastSeenAt = device.LastSeenAt
        };
    }


    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, DeviceUpdateDto dto)
    {
        var device = await _deviceService.GetByIdAsync(id);
        if (device == null) return NotFound();

        device.IsActive = dto.IsActive;
        device.LastSeenAt = DateTime.UtcNow;

        await _deviceService.UpdateAsync(device);
        return NoContent();
    }


    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var device = await _deviceService.GetByIdAsync(id);
        if (device == null) return NotFound();

        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var isAdmin = User.IsInRole("Admin");

        if (!isAdmin && device.UserId != userId)
            return Forbid();

        await _deviceService.DeleteAsync(id);
        return NoContent();
    }
}
