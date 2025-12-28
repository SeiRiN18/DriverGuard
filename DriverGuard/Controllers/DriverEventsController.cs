using DriverGuard.Backend.DTO.DriverEvent;
using DriverGuard.Backend.Models;
using DriverGuard.Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/events")]
public class DriverEventsController : ControllerBase
{
    private readonly IDriverEventService _eventService;

    public DriverEventsController(IDriverEventService eventService)
    {
        _eventService = eventService;
    }

    // ==================================================
    // CREATE: IoT-ПРИСТРІЙ надсилає подію
    // АВТОРИЗАЦІЯ: X-Device-Key (через middleware)
    // ==================================================
    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Create(DriverEventCreateDto dto)
    {
        if (!HttpContext.Items.ContainsKey("DeviceId"))
            return Unauthorized("Device authentication required");

        var deviceId = (Guid)HttpContext.Items["DeviceId"];

        var ev = new DriverEvent
        {
            Id = Guid.NewGuid(),
            DeviceId = deviceId,
            EventType = dto.EventType,
            Severity = dto.Severity,
            Confidence = dto.Confidence,
            OccurredAt = dto.OccurredAt,
            CreatedAt = DateTime.UtcNow
        };

        await _eventService.CreateAsync(ev);
        return Ok();
    }


    // ==================================================
    // READ ALL: ADMIN
    // ==================================================
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<IEnumerable<DriverEventReadDto>>> GetAll()
    {
        var events = await _eventService.GetAllAsync();

        return events
            .OrderByDescending(e => e.OccurredAt)
            .Select(e => new DriverEventReadDto
            {
                Id = e.Id,
                DeviceId = e.DeviceId,
                EventType = e.EventType,
                Severity = e.Severity,
                Confidence = e.Confidence,
                OccurredAt = e.OccurredAt
            })
            .ToList();
    }

    // ==================================================
    // READ BY DEVICE: USER або ADMIN
    // ==================================================
    [HttpGet("device/{deviceId}")]
    [Authorize]
    public async Task<ActionResult<IEnumerable<DriverEventReadDto>>> GetByDevice(Guid deviceId)
    {
        var events = await _eventService.GetByDeviceIdAsync(deviceId);

        return events
            .OrderByDescending(e => e.OccurredAt)
            .Select(e => new DriverEventReadDto
            {
                Id = e.Id,
                DeviceId = e.DeviceId,
                EventType = e.EventType,
                Severity = e.Severity,
                Confidence = e.Confidence,
                OccurredAt = e.OccurredAt
            })
            .ToList();
    }
}
