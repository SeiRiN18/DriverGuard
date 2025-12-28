namespace DriverGuard.DTO.Admin
{
    public class AdminStatsDto
    {
        public int Users { get; set; }
        public int Devices { get; set; }
        public int Events { get; set; }
        public int CriticalEvents { get; set; }
        public int Notifications { get; set; }
        public int UnreadNotifications { get; set; }
    }
}
