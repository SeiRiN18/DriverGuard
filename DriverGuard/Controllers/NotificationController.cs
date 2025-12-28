using DriverGuard.DTO.Notification;
using DriverGuard.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[ApiController]
[Route("api/notifications")]
[Authorize] // 🔒 Всі ендпоінти захищені
public class NotificationsController : ControllerBase
{
    private readonly INotificationService _notificationService;

    public NotificationsController(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    // ==========================================
    // GET: МОЇ СПОВІЩЕННЯ (USER)
    // ==========================================
    [HttpGet("my")]
    public async Task<ActionResult<IEnumerable<NotificationReadDto>>> GetMy()
    {
        var userId = Guid.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!
        );

        var notifications = await _notificationService.GetByUserIdAsync(userId);

        return notifications.Select(n => new NotificationReadDto
        {
            Id = n.Id,
            DeviceId = n.DeviceId,
            DriverEventId = n.DriverEventId,
            Type = n.Type,
            Message = n.Message,
            IsRead = n.IsRead,
            CreatedAt = n.CreatedAt
        }).ToList();
    }

    // ==========================================
    // GET: ПО ПРИСТРОЮ (ADMIN)
    // ==========================================
    [HttpGet("device/{deviceId}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<IEnumerable<NotificationReadDto>>> GetByDevice(Guid deviceId)
    {
        var notifications = await _notificationService.GetByDeviceIdAsync(deviceId);

        return notifications.Select(n => new NotificationReadDto
        {
            Id = n.Id,
            DeviceId = n.DeviceId,
            DriverEventId = n.DriverEventId,
            Type = n.Type,
            Message = n.Message,
            IsRead = n.IsRead,
            CreatedAt = n.CreatedAt
        }).ToList();
    }

    // ==========================================
    // PUT: ПОЗНАЧИТИ ЯК ПРОЧИТАНЕ (USER)
    // ==========================================
    [HttpPut("{id}/read")]
    public async Task<IActionResult> MarkAsRead(Guid id)
    {
        var userId = Guid.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!
        );

        var success = await _notificationService.MarkAsReadAsync(id, userId);

        if (!success)
            return Forbid();

        return NoContent();
    }
}
