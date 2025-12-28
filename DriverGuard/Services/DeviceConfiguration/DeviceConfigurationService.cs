using DriverGuard.Backend.Data;
using DriverGuard.Backend.Exceptions;
using DriverGuard.Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace DriverGuard.Backend.Services
{
    public class DeviceConfigurationService : IDeviceConfigurationService
    {
        private readonly DriverGuardDbContext _context;

        public DeviceConfigurationService(DriverGuardDbContext context)
        {
            _context = context;
        }

        // =====================================================
        // VALIDATION (BUSINESS RULES)
        // =====================================================
        private void ValidateConfiguration(DeviceConfiguration config)
        {
            if (config == null)
                throw new ValidationException("Конфігурація не може бути порожньою");

            if (config.DeviceId == Guid.Empty)
                throw new ValidationException("DeviceId є обовʼязковим");

            if (config.DrowsinessThreshold < 0 || config.DrowsinessThreshold > 1)
                throw new ValidationException(
                    "DrowsinessThreshold повинен бути в межах 0..1");

            if (config.AttentionThreshold < 0 || config.AttentionThreshold > 1)
                throw new ValidationException(
                    "AttentionThreshold повинен бути в межах 0..1");
        }

        // =====================================================
        // READ CONFIGURATION
        // =====================================================
        public async Task<DeviceConfiguration?> GetByDeviceIdAsync(Guid deviceId)
        {
            if (deviceId == Guid.Empty)
                throw new ValidationException("DeviceId не може бути порожнім");

            return await _context.DeviceConfigurations
                .FirstOrDefaultAsync(c => c.DeviceId == deviceId);
        }

        // =====================================================
        // CREATE OR UPDATE CONFIGURATION (BUSINESS LOGIC)
        // =====================================================
        public async Task<DeviceConfiguration> CreateOrUpdateAsync(
            DeviceConfiguration config)
        {
            ValidateConfiguration(config);

            var existing = await _context.DeviceConfigurations
                .FirstOrDefaultAsync(c => c.DeviceId == config.DeviceId);

            if (existing == null)
            {
                config.UpdatedAt = DateTime.UtcNow;
                _context.DeviceConfigurations.Add(config);
            }
            else
            {
                existing.DrowsinessThreshold = config.DrowsinessThreshold;
                existing.AttentionThreshold = config.AttentionThreshold;
                existing.UpdatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
            return config;
        }
    }
}
