using DriverGuard.Backend.Data;
using DriverGuard.Backend.Middleware;
using DriverGuard.Backend.Services;
using DriverGuard.Backend.Services.AdminStats;
using DriverGuard.Backend.Services.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);


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
        builder.Services.AddScoped<IAdminStatsService, AdminStatsService>();
        builder.Services.AddScoped<IJwtService, JwtService>();

        builder.Services.AddEndpointsApiExplorer();

        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "DriverGuard API",
                Version = "v1",
                Description = "API для системи моніторингу водіїв"
            });

            // Добавляем JWT авторизацию в Swagger
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Введіть JWT токен у форматі: Bearer {ваш токен}"
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
                Description = "API key для IoT-пристрою (X-Device-Key)",
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

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }


        app.UseMiddleware<ExceptionHandlingMiddleware>();
        app.UseMiddleware<DeviceAuthMiddleware>();

        app.UseAuthentication();

        app.UseAuthorization();



        app.MapControllers();

        app.Run();
    }
}