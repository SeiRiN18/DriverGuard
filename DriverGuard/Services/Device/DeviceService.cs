using DriverGuard.Data;
using DriverGuard.Exceptions;
using DriverGuard.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace DriverGuard.Services
{
    public class DeviceService : IDeviceService
    {
        private readonly DriverGuardDbContext _context;

        public DeviceService(DriverGuardDbContext context)
        {
            _context = context;
        }

        // =====================================================
        // CREATE DEVICE (BUSINESS LOGIC)
        // =====================================================
        public async Task<(Device device, string apiKey)> CreateAsync(Device device)

        {
            if (device == null)
                throw new ValidationException("Пристрій не може бути порожнім");

            if (device.UserId == Guid.Empty)
                throw new ValidationException("UserId є обовʼязковим");

            if (string.IsNullOrWhiteSpace(device.SerialNumber))
                throw new ValidationException("SerialNumber є обовʼязковим");

            var exists = await _context.Devices
                .AnyAsync(d => d.SerialNumber == device.SerialNumber);

            if (exists)
                throw new ValidationException(
                    "Пристрій з таким SerialNumber вже існує");

            var rawKey = Guid.NewGuid().ToString();

            device.Id = Guid.NewGuid();
            device.ApiKeyHash = Hash(rawKey); // 👈 зберігаємо ТІЛЬКИ ХЕШ
            device.IsActive = true;
            device.CreatedAt = DateTime.UtcNow;
            device.LastSeenAt = DateTime.UtcNow;

            _context.Devices.Add(device);
            await _context.SaveChangesAsync();

            _context.DeviceConfigurations.Add(new DeviceConfiguration
            {
                DeviceId = device.Id,
                DrowsinessThreshold = 0.6,
                AttentionThreshold = 0.6,
                UpdatedAt = DateTime.UtcNow
            });
            await _context.SaveChangesAsync();

            // ⚠️ ПОВЕРТАЄМО RAW KEY ОДИН РАЗ
            return (device, rawKey);
        }

        // =====================================================
        // READ DEVICE BY ID
        // =====================================================
        public async Task<Device?> GetByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new ValidationException("Id не може бути порожнім");

            return await _context.Devices
                .Include(d => d.DeviceConfiguration)
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        // =====================================================
        // READ DEVICES BY USER
        // =====================================================
        public async Task<IEnumerable<Device>> GetByUserIdAsync(Guid userId)
        {
            if (userId == Guid.Empty)
                throw new ValidationException("UserId не може бути порожнім");

            return await _context.Devices
                .Where(d => d.UserId == userId)
                .ToListAsync();
        }

        // =====================================================
        // READ ALL DEVICES (ADMIN)
        // =====================================================
        public async Task<IEnumerable<Device>> GetAllAsync()
        {
            return await _context.Devices.ToListAsync();
        }

        // =====================================================
        // UPDATE DEVICE STATUS
        // =====================================================
        public async Task UpdateAsync(Device device)
        {
            if (device.Id == Guid.Empty)
                throw new ValidationException("Id є обовʼязковим");

            var existing = await _context.Devices
                .FirstOrDefaultAsync(d => d.Id == device.Id);

            if (existing == null)
                throw new NotFoundException("Пристрій не знайдено");

            existing.IsActive = device.IsActive;
            existing.LastSeenAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }

        // =====================================================
        // DELETE DEVICE
        // =====================================================
        public async Task DeleteAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new ValidationException("Id не може бути порожнім");

            var device = await _context.Devices.FindAsync(id);
            if (device == null)
                throw new NotFoundException("Пристрій не знайдено");

            _context.Devices.Remove(device);
            await _context.SaveChangesAsync();
        }

        // =====================================================
        // API KEY HASH GENERATION
        // =====================================================
        private static string Hash(string input)
        {
            using var sha = SHA256.Create();
            return Convert.ToBase64String(
                sha.ComputeHash(Encoding.UTF8.GetBytes(input))
            );
        }
    }
}
