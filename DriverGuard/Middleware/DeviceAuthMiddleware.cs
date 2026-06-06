using DriverGuard.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace DriverGuard.Middleware
{
    public class DeviceAuthMiddleware
    {
        private readonly RequestDelegate _next;

        public DeviceAuthMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, DriverGuardDbContext db)
        {
            var isIotPost = context.Request.Method == "POST"
                && context.Request.Path.Equals("/api/events", StringComparison.OrdinalIgnoreCase);

            if (!isIotPost)
            {
                await _next(context);
                return;
            }

            if (!context.Request.Headers.TryGetValue("X-Device-Key", out var rawKey))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return;
            }

            var hashedKey = Hash(rawKey!);

            var device = await db.Devices
                .FirstOrDefaultAsync(d => d.ApiKeyHash == hashedKey && d.IsActive);

            if (device == null)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return;
            }

            // Кладемо DeviceId у контекст
            context.Items["DeviceId"] = device.Id;

            await _next(context);
        }

        private static string Hash(string input)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
            return Convert.ToBase64String(bytes);
        }
    }
}
