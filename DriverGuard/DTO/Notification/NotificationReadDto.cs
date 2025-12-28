namespace DriverGuard.DTO.Notification
{
    public class NotificationReadDto
    {
        public Guid Id { get; set; }
        public Guid DeviceId { get; set; }
        public Guid DriverEventId { get; set; }
        public string Type { get; set; } = null!;
        public string Message { get; set; } = null!;
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
