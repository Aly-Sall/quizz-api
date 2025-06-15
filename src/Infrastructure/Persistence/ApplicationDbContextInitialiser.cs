// src/Infrastructure/Persistence/ApplicationDbContextInitialiser.cs
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using _Net6CleanArchitectureQuizzApp.Domain.Entities;

namespace _Net6CleanArchitectureQuizzApp.Infrastructure.Persistence;

public class ApplicationDbContextInitialiser
{
    private readonly ILogger<ApplicationDbContextInitialiser> _logger;
    private readonly ApplicationDbContext _context;
    private readonly UserManager<User> _userManager;
    // ✅ Utiliser IdentityRole au lieu de IdentityRole<int>
    private readonly RoleManager<IdentityRole<int>> _roleManager;

    public ApplicationDbContextInitialiser(
         ILogger<ApplicationDbContextInitialiser> logger,
        ApplicationDbContext context,
        UserManager<User> userManager,
        RoleManager<IdentityRole<int>> roleManager)
    {
        _logger = logger;
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task InitialiseAsync()
    {
        try
        {
            _logger.LogInformation("🔍 Starting database initialization...");

            // Créer la base de données si elle n'existe pas
            if (await _context.Database.CanConnectAsync())
            {
                _logger.LogInformation("✅ Database connection successful");
            }
            else
            {
                _logger.LogInformation("🔍 Creating database...");
                await _context.Database.EnsureCreatedAsync();
                _logger.LogInformation("✅ Database created successfully");
            }

            // Appliquer les migrations en attente
            if ((await _context.Database.GetPendingMigrationsAsync()).Any())
            {
                _logger.LogInformation("🔍 Applying pending migrations...");
                await _context.Database.MigrateAsync();
                _logger.LogInformation("✅ Migrations applied successfully");
            }
            else
            {
                _logger.LogInformation("✅ Database is up to date");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ An error occurred while initialising the database");
            throw;
        }
    }

    public async Task SeedAsync()
    {
        try
        {
            _logger.LogInformation("🔍 Starting database seeding...");

            // ✅ Créer des rôles de base (optionnel)
            await TrySeedRolesAsync();

            // ✅ Créer un utilisateur administrateur de test (optionnel)
            await TrySeedDefaultUserAsync();

            _logger.LogInformation("✅ Database seeding completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ An error occurred while seeding the database");
            // Ne pas lever l'exception pour éviter de bloquer l'application
        }
    }

    private async Task TrySeedRolesAsync()
    {
        try
        {
            var roles = new[] { "Administrator", "User", "Candidate" };

            foreach (var roleName in roles)
            {
                if (!await _roleManager.RoleExistsAsync(roleName))
                {
                    _logger.LogInformation("🔍 Creating role: {RoleName}", roleName);
                    await _roleManager.CreateAsync(new IdentityRole<int>(roleName));
                    _logger.LogInformation("✅ Role created: {RoleName}", roleName);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "⚠️ Could not seed roles, continuing without them");
        }
    }

    private async Task TrySeedDefaultUserAsync()
    {
        try
        {
            var defaultEmail = "admin@example.com";
            var defaultUser = await _userManager.FindByEmailAsync(defaultEmail);

            if (defaultUser == null)
            {
                _logger.LogInformation("🔍 Creating default admin user...");

                defaultUser = new User
                {
                    UserName = defaultEmail,
                    Email = defaultEmail,
                    Nom = "Admin",
                    Prenom = "System",
                    EmailConfirmed = true
                };

                var result = await _userManager.CreateAsync(defaultUser, "Admin123!");

                if (result.Succeeded)
                {
                    _logger.LogInformation("✅ Default admin user created: {Email}", defaultEmail);

                    // Ajouter au rôle Administrator si possible
                    try
                    {
                        if (await _roleManager.RoleExistsAsync("Administrator"))
                        {
                            await _userManager.AddToRoleAsync(defaultUser, "Administrator");
                            _logger.LogInformation("✅ Admin user added to Administrator role");
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "⚠️ Could not add user to Administrator role");
                    }
                }
                else
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    _logger.LogWarning("⚠️ Could not create default admin user: {Errors}", errors);
                }
            }
            else
            {
                _logger.LogInformation("✅ Default admin user already exists");
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "⚠️ Could not seed default user, continuing without it");
        }
    }
}