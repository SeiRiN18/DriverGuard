namespace DriverGuard.Models
{
    public class User
    {
        public Guid Id { get; set;  }
        public string Email { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public UserRole Role { get; set; }
        public ICollection<Device> Devices { get; set; } = new List<Device>();
        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    }
}
