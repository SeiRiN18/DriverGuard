using DriverGuard.Models;

public interface IJwtService
{
    string GenerateToken(User user);
}
