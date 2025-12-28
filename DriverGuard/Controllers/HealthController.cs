using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DriverGuard.Data;

[ApiController]
[Route("api/health")]
public class HealthController : ControllerBase
{
    private readonly DriverGuardDbContext _context;

    public HealthController(DriverGuardDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var canConnect = await _context.Database.CanConnectAsync();

        return Ok(new
        {
            status = "OK",
            database = canConnect ? "connected" : "unavailable",
            timestamp = DateTime.UtcNow
        });
    }
}
