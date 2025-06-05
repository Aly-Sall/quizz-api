// src/Infrastructure/InfrastructureServices.cs
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using _Net6CleanArchitectureQuizzApp.Application.Common.Interfaces;
using _Net6CleanArchitectureQuizzApp.Domain.Entities;
using _Net6CleanArchitectureQuizzApp.Domain.Interfaces;
using _Net6CleanArchitectureQuizzApp.Infrastructure.Persistence;
using _Net6CleanArchitectureQuizzApp.Infrastructure.Services;
namespace Microsoft.Extensions.DependencyInjection;

public static class InfrastructureServices
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        // ✅ Configuration de la base de données
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());

        // ✅ Configuration d'Identity - VERSION SIMPLIFIÉE
        services.AddDefaultIdentity<User>(options =>
        {
            // Options de mot de passe très permissives
            options.Password.RequireDigit = false;
            options.Password.RequiredLength = 4;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireLowercase = false;
            options.Password.RequiredUniqueChars = 0;

            // Configuration utilisateur
            options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
            options.User.RequireUniqueEmail = true;

            // Désactiver le verrouillage
            options.Lockout.AllowedForNewUsers = false;
            options.Lockout.MaxFailedAccessAttempts = 999;

            // Pas de confirmation requise
            options.SignIn.RequireConfirmedEmail = false;
            options.SignIn.RequireConfirmedPhoneNumber = false;
            options.SignIn.RequireConfirmedAccount = false;
        })
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();

        // ✅ IMPORTANT: Enregistrer explicitement les gestionnaires Identity
        services.AddScoped<UserManager<User>>();
        services.AddScoped<SignInManager<User>>();

        // ✅ Enregistrer RoleManager avec le type par défaut (IdentityRole au lieu de IdentityRole<int>)
        services.AddScoped<RoleManager<IdentityRole>>();

        // ✅ Services d'application
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IEmailService, ApplicationEmailService>();

        // ✅ JWT Token Generator simple
        services.AddScoped<IJwtTokenGenerator, SimpleJwtTokenGenerator>();

        // ✅ Initialisation de la base de données (version simplifiée)
        services.AddScoped<ApplicationDbContextInitialiser>();

        return services;
    }
}

// ✅ Implémentation simple du générateur JWT
public class SimpleJwtTokenGenerator : IJwtTokenGenerator
{
    public (string Token, DateTime Expiry) GenerateToken(User user)
    {
        var expiry = DateTime.UtcNow.AddHours(24);
        var token = $"simple-token-{user.Id}-{DateTime.UtcNow.Ticks}";
        return (token, expiry);
    }

    public System.Security.Claims.ClaimsPrincipal? GetPrincipalFromToken(string token)
    {
        return null;
    }
}