// src/Infrastructure/InfrastructureServices.cs - Configuration complète
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
        // Base de données
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());
        services.AddScoped<ApplicationDbContextInitialiser>();

        // ✅ Configuration Identity avec l'entité User personnalisée
        services.AddDefaultIdentity<User>(options =>
        {
            options.SignIn.RequireConfirmedAccount = false;
            options.Password.RequireDigit = true;
            options.Password.RequiredLength = 6;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = true;
            options.Password.RequireLowercase = true;
        })
        .AddRoles<IdentityRole<int>>()
        .AddEntityFrameworkStores<ApplicationDbContext>();

        // ✅ Configuration JWT Authentication
        var jwtSecret = configuration["Jwt:Secret"] ?? "YourSuperSecretKeyThatIsAtLeast32CharactersLong!";
        var key = Encoding.ASCII.GetBytes(jwtSecret);

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
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
                ClockSkew = TimeSpan.Zero
            };
        });

        // ✅ Enregistrer les services manquants
        services.AddScoped<IIdentityService, IdentityService>();
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

        // ✅ Autres services d'infrastructure
        services.AddScoped<IDateTime, DateTimeService>();

        return services;
    }
}

// Service DateTime simple
public class DateTimeService : IDateTime
{
    public DateTime Now => DateTime.Now;
}