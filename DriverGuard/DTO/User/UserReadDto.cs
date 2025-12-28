namespace DriverGuard.DTO.User
{
    public class UserReadDto
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
    }

}
