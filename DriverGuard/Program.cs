using DriverGuard.Data;
using DriverGuard.Middleware;
using DriverGuard.Services;
using DriverGuard.Services.AdminStats;
using DriverGuard.Services.Fcm;
using DriverGuard.Services.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(new WebApplicationOptions
        {
            Args = args
        });

        builder.Configuration.GetSection("hostBuilder:reloadConfigOnChange").Value = "false";

        builder.Host.ConfigureAppConfiguration((hostingContext, config) =>
        {
            foreach (var source in config.Sources.OfType<FileConfigurationSource>())
            {
                source.ReloadOnChange = false;
            }
        });


        builder.Services.AddControllers();
        builder.Services.AddOpenApi();

        builder.Services.AddDbContext<DriverGuardDbContext>(options =>
            options.UseNpgsql(
                builder.Configuration.GetConnectionString("DefaultConnection")
            )
        );

        builder.Services.AddScoped<IUserService, UserService>();
        builder.Services.AddScoped<IDeviceService, DeviceService>();
        builder.Services.AddScoped<IDeviceConfigurationService, DeviceConfigurationService>();
        builder.Services.AddScoped<IDriverEventService, DriverEventService>();
        builder.Services.AddScoped<INotificationService, NotificationService>();
        builder.Services.AddSingleton<IFcmService, FcmService>();
        builder.Services.AddScoped<IAdminStatsService, AdminStatsService>();
        builder.Services.AddScoped<IJwtService, JwtService>();

        builder.Services.AddEndpointsApiExplorer();

        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "DriverGuard API",
                Version = "v1",
                Description = "API for DriverGuard application"
            });


            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "JWT: Bearer {}"
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
            });
        });
        builder.Services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("DeviceKey", new OpenApiSecurityScheme
            {
                Description = "API key IoT (X-Device-Key)",
                Name = "X-Device-Key",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "DeviceKey"
                }
            },
            Array.Empty<string>()
        }
    });
        });


        builder.Services.AddAuthentication("Bearer")
           .AddJwtBearer("Bearer", options =>
           {
               options.TokenValidationParameters = new TokenValidationParameters
               {
                   ValidateIssuer = true,
                   ValidateAudience = true,
                   ValidateLifetime = true,
                   ValidateIssuerSigningKey = true,
                   ValidIssuer = builder.Configuration["Jwt:Issuer"],
                   ValidAudience = builder.Configuration["Jwt:Audience"],
                   IssuerSigningKey = new SymmetricSecurityKey(
                       Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)
                   )
               };
           });


        builder.Services.AddAuthorization();




        var app = builder.Build();


        app.UseSwagger();
        app.UseSwaggerUI();


        using (var scope = app.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<DriverGuardDbContext>();
            db.Database.Migrate();

            var adminEmail    = builder.Configuration["ADMIN_EMAIL"];
            var adminPassword = builder.Configuration["ADMIN_PASSWORD"];

            if (!string.IsNullOrWhiteSpace(adminEmail) && !string.IsNullOrWhiteSpace(adminPassword))
            {
                var exists = db.Users.Any(u => u.Role == DriverGuard.Models.UserRole.Admin);
                if (!exists)
                {
                    db.Users.Add(new DriverGuard.Models.User
                    {
                        Id           = Guid.NewGuid(),
                        Email        = adminEmail,
                        PasswordHash = BCrypt.Net.BCrypt.HashPassword(adminPassword),
                        Role         = DriverGuard.Models.UserRole.Admin,
                        CreatedAt    = DateTime.UtcNow
                    });
                    db.SaveChanges();
                }
            }

            var devicesWithoutConfig = db.Devices
                .Where(d => !db.DeviceConfigurations.Any(c => c.DeviceId == d.Id))
                .Select(d => d.Id)
                .ToList();

            foreach (var deviceId in devicesWithoutConfig)
            {
                db.DeviceConfigurations.Add(new DriverGuard.Models.DeviceConfiguration
                {
                    DeviceId             = deviceId,
                    DrowsinessThreshold  = 0.6,
                    AttentionThreshold   = 0.6,
                    UpdatedAt            = DateTime.UtcNow
                });
            }
            if (devicesWithoutConfig.Count > 0)
                db.SaveChanges();
        }



        app.UseMiddleware<ExceptionHandlingMiddleware>();
        app.UseMiddleware<DeviceAuthMiddleware>();

        app.UseAuthentication();

        app.UseAuthorization();



        app.MapControllers();

        app.Run();
    }
}