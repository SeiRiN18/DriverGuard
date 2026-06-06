using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using DriverGuard.Services.Fcm;

namespace DriverGuard.Services.Fcm
{
    public class FcmService : IFcmService
    {
        private readonly ILogger<FcmService> _logger;
        private readonly bool _initialized;

        public FcmService(IConfiguration configuration, ILogger<FcmService> logger)
        {
            _logger = logger;

            if (FirebaseApp.DefaultInstance != null)
            {
                _initialized = true;
                return;
            }

            var serviceAccountJson = configuration["Firebase:ServiceAccountJson"];
            if (string.IsNullOrWhiteSpace(serviceAccountJson))
            {
                _logger.LogWarning("Firebase:ServiceAccountJson is not configured — push notifications disabled.");
                return;
            }

            FirebaseApp.Create(new AppOptions
            {
                Credential = GoogleCredential.FromJson(serviceAccountJson)
            });

            _initialized = true;
        }

        public async Task SendAsync(
            string fcmToken,
            string title,
            string body,
            Dictionary<string, string>? data = null)
        {
            if (!_initialized || string.IsNullOrWhiteSpace(fcmToken))
                return;

            try
            {
                var message = new Message
                {
                    Token = fcmToken,
                    Notification = new Notification { Title = title, Body = body },
                    Data = data ?? new Dictionary<string, string>(),
                    Android = new AndroidConfig
                    {
                        Priority = Priority.High,
                        Notification = new AndroidNotification
                        {
                            ChannelId = "driverguard_alerts",
                            Sound = "default"
                        }
                    }
                };

                await FirebaseMessaging.DefaultInstance.SendAsync(message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send FCM push to token {Token}", fcmToken[..8] + "...");
            }
        }
    }
}
