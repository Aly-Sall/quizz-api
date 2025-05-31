// src/Infrastructure/InfrastructureServices.cs - Configuration complète avec EmailService
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using _Net6CleanArchitectureQuizzApp.Application.Common.Interfaces;
using _Net6CleanArchitectureQuizzApp.Infrastructure.Persistence;
using _Net6CleanArchitectureQuizzApp.Infrastructure.Services;
using _Net6CleanArchitectureQuizzApp.Domain.Entities;
using _Net6CleanArchitectureQuizzApp.Domain.Interfaces;

namespace Microsoft.Extensions.DependencyInjection;

public static class InfrastructureServices
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        // ================================
        // BASE DE DONNÉES
        // ================================
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IApplicationDbContext>(provider =>
            provider.GetRequiredService<ApplicationDbContext>());

        services.AddScoped<ApplicationDbContextInitialiser>();

        // ================================
        // IDENTITY & AUTHENTICATION
        // ================================

        // Configuration Identity avec l'entité User personnalisée
        services.AddDefaultIdentity<User>(options =>
        {
            // Paramètres de connexion
            options.SignIn.RequireConfirmedAccount = false;
            options.SignIn.RequireConfirmedEmail = false;

            // Paramètres de mot de passe
            options.Password.RequireDigit = true;
            options.Password.RequiredLength = 6;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = true;
            options.Password.RequireLowercase = true;

            // Paramètres de verrouillage
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.AllowedForNewUsers = true;

            // Paramètres utilisateur
            options.User.RequireUniqueEmail = true;
        })
        .AddRoles<IdentityRole<int>>()
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();

        // Configuration JWT Authentication
        var jwtSecret = configuration["Jwt:Secret"] ?? "YourSuperSecretKeyThatIsAtLeast32CharactersLong!";
        var key = Encoding.ASCII.GetBytes(jwtSecret);

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = false;
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
        });

        // ================================
        // SERVICES D'APPLICATION
        // ================================

        // Services d'authentification et d'identité
        services.AddScoped<IIdentityService, IdentityService>();
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

        // ✅ NOUVEAU: Service d'envoi d'emails
        services.AddScoped<IEmailService, EmailService>();

        // Services utilitaires
        services.AddScoped<IDateTime, DateTimeService>();

        // ✅ NOUVEAU: Service de gestion des fichiers (si nécessaire)
        // services.AddScoped<IFileStorageService, FileStorageService>();

        // ✅ NOUVEAU: Service de cache (optionnel)
        // services.AddMemoryCache();
        // services.AddScoped<ICacheService, CacheService>();

        // ================================
        // CONFIGURATION SUPPLÉMENTAIRE
        // ================================

        // Configuration des cookies d'authentification
        services.ConfigureApplicationCookie(options =>
        {
            options.LoginPath = "/Account/Login";
            options.LogoutPath = "/Account/Logout";
            options.AccessDeniedPath = "/Account/AccessDenied";
            options.ExpireTimeSpan = TimeSpan.FromDays(7);
            options.SlidingExpiration = true;
        });

        return services;
    }
}

// ================================
// SERVICES D'INFRASTRUCTURE
// ================================

/// <summary>
/// Service pour obtenir la date et l'heure actuelles
/// </summary>
public class DateTimeService : IDateTime
{
    public DateTime Now => DateTime.Now;
}

/// <summary>
/// Service de gestion des fichiers (optionnel)
/// </summary>
/*
public interface IFileStorageService
{
    Task<string> SaveFileAsync(Stream fileStream, string fileName, string directory = "uploads");
    Task<bool> DeleteFileAsync(string filePath);
    Task<Stream> GetFileAsync(string filePath);
}

public class FileStorageService : IFileStorageService
{
    private readonly IWebHostEnvironment _environment;
    
    public FileStorageService(IWebHostEnvironment environment)
    {
        _environment = environment;
    }
    
    public async Task<string> SaveFileAsync(Stream fileStream, string fileName, string directory = "uploads")
    {
        var uploadsPath = Path.Combine(_environment.WebRootPath, directory);
        Directory.CreateDirectory(uploadsPath);
        
        var uniqueFileName = $"{Guid.NewGuid()}_{fileName}";
        var filePath = Path.Combine(uploadsPath, uniqueFileName);
        
        using var fileStreamOutput = new FileStream(filePath, FileMode.Create);
        await fileStream.CopyToAsync(fileStreamOutput);
        
        return Path.Combine(directory, uniqueFileName);
    }
    
    public Task<bool> DeleteFileAsync(string filePath)
    {
        var fullPath = Path.Combine(_environment.WebRootPath, filePath);
        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
            return Task.FromResult(true);
        }
        return Task.FromResult(false);
    }
    
    public Task<Stream> GetFileAsync(string filePath)
    {
        var fullPath = Path.Combine(_environment.WebRootPath, filePath);
        return Task.FromResult<Stream>(new FileStream(fullPath, FileMode.Open, FileAccess.Read));
    }
}
*/

/// <summary>
/// Service de cache en mémoire (optionnel)
/// </summary>
/*
public interface ICacheService
{
    Task<T?> GetAsync<T>(string key) where T : class;
    Task SetAsync<T>(string key, T value, TimeSpan? expiration = null) where T : class;
    Task RemoveAsync(string key);
}

public class CacheService : ICacheService
{
    private readonly IMemoryCache _cache;
    
    public CacheService(IMemoryCache cache)
    {
        _cache = cache;
    }
    
    public Task<T?> GetAsync<T>(string key) where T : class
    {
        _cache.TryGetValue(key, out var value);
        return Task.FromResult(value as T);
    }
    
    public Task SetAsync<T>(string key, T value, TimeSpan? expiration = null) where T : class
    {
        var options = new MemoryCacheEntryOptions();
        if (expiration.HasValue)
            options.SetAbsoluteExpiration(expiration.Value);
        else
            options.SetAbsoluteExpiration(TimeSpan.FromMinutes(30)); // Expiration par défaut
            
        _cache.Set(key, value, options);
        return Task.CompletedTask;
    }
    
    public Task RemoveAsync(string key)
    {
        _cache.Remove(key);
        return Task.CompletedTask;
    }
}
*/