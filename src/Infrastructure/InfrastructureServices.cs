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
using _Net6CleanArchitectureQuizzApp.Infrastructure.Settings;
using _Net6CleanArchitectureQuizzApp.Infrastructure.Identity; // ← AJOUTÉ pour IdentityService
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection;

public static class InfrastructureServices
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        // ✅ Configuration de la base de données
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());

        // ✅ Configuration JWT Settings
        var jwtSettings = new JwtSettings();
        configuration.Bind(JwtSettings.SectionName, jwtSettings);
        services.AddSingleton(Microsoft.Extensions.Options.Options.Create(jwtSettings));

        // ✅ Configuration d'Identity - VERSION SIMPLIFIÉE SANS RÔLES
        services.AddIdentity<User, IdentityRole<int>>(options =>
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

        // ✅ Configuration de l'authentification JWT
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings.Issuer,
                ValidAudience = jwtSettings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret!)),
                ClockSkew = TimeSpan.Zero
            };
        });

        // ✅ Services d'application
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IIdentityService, _Net6CleanArchitectureQuizzApp.Infrastructure.Identity.IdentityService>(); // ← AJOUTÉ cette ligne

        // ✅ JWT Token Generator CORRIGÉ
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

        // ✅ Initialisation de la base de données
        services.AddScoped<ApplicationDbContextInitialiser>();

        return services;
    }
}