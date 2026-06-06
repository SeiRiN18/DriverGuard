namespace DriverGuard.Services.Fcm
{
    public interface IFcmService
    {
        Task SendAsync(string fcmToken, string title, string body, Dictionary<string, string>? data = null);
    }
}
