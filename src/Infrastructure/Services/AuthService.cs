// src/Infrastructure/Services/AuthService.cs
using System;
using System.Threading.Tasks;
using _Net6CleanArchitectureQuizzApp.Application.Common.Interfaces;
using _Net6CleanArchitectureQuizzApp.Domain.Entities;
using _Net6CleanArchitectureQuizzApp.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace _Net6CleanArchitectureQuizzApp.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly IJwtTokenGenerator _tokenGenerator;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        IJwtTokenGenerator tokenGenerator,
        ILogger<AuthService> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenGenerator = tokenGenerator;
        _logger = logger;
    }

    public async Task<AuthResult> LoginAsync(string email, string password)
    {
        try
        {
            _logger.LogInformation("🔍 AuthService: Attempting login for {Email}", email);

            var normalizedEmail = email.ToLower().Trim();

            // Chercher l'utilisateur
            var user = await _userManager.FindByEmailAsync(normalizedEmail)
                      ?? await _userManager.FindByNameAsync(normalizedEmail);

            if (user == null)
            {
                _logger.LogWarning("❌ User not found: {Email}", normalizedEmail);
                return AuthResult.Failure("Email ou mot de passe incorrect");
            }

            _logger.LogInformation("✅ User found: ID={Id}, Email={UserEmail}", user.Id, user.Email);

            // Vérifier le mot de passe avec SignInManager
            var signInResult = await _signInManager.CheckPasswordSignInAsync(user, password, false);

            if (signInResult.Succeeded)
            {
                _logger.LogInformation("✅ SignIn successful via SignInManager");
                var (token, expiry) = _tokenGenerator.GenerateToken(user);

                return AuthResult.Success(
                    token: token,
                    expiry: expiry,
                    email: user.Email!,
                    userName: user.UserName,
                    nom: user.Nom,
                    prenom: user.Prenom,
                    userId: user.Id
                );
            }

            // Fallback: vérifier avec UserManager direct
            _logger.LogInformation("🔍 SignInManager failed, trying UserManager fallback...");
            var passwordValid = await _userManager.CheckPasswordAsync(user, password);

            if (passwordValid)
            {
                _logger.LogInformation("✅ Password valid via UserManager fallback");
                var (token, expiry) = _tokenGenerator.GenerateToken(user);

                return AuthResult.Success(
                    token: token,
                    expiry: expiry,
                    email: user.Email!,
                    userName: user.UserName,
                    nom: user.Nom,
                    prenom: user.Prenom,
                    userId: user.Id
                );
            }

            _logger.LogWarning("❌ Password verification failed for user: {Email}", normalizedEmail);
            _logger.LogInformation("🔍 SignInResult: IsLockedOut={IsLockedOut}, IsNotAllowed={IsNotAllowed}, RequiresTwoFactor={RequiresTwoFactor}",
                signInResult.IsLockedOut, signInResult.IsNotAllowed, signInResult.RequiresTwoFactor);

            return AuthResult.Failure("Email ou mot de passe incorrect");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error during login for: {Email}", email);
            return AuthResult.Failure("Une erreur s'est produite lors de la connexion");
        }
    }

    public async Task<AuthResult> RegisterAsync(string email, string password, string? nom = null, string? prenom = null)
    {
        try
        {
            _logger.LogInformation("🔍 AuthService: Attempting registration for {Email}", email);

            var normalizedEmail = email.ToLower().Trim();

            // Créer l'utilisateur
            var user = new User
            {
                Email = normalizedEmail,
                UserName = normalizedEmail,
                NormalizedEmail = normalizedEmail.ToUpperInvariant(),
                NormalizedUserName = normalizedEmail.ToUpperInvariant(),
                Nom = nom?.Trim(),
                Prenom = prenom?.Trim(),
                EmailConfirmed = true,
                LockoutEnabled = false,
                AccessFailedCount = 0,
                TwoFactorEnabled = false,
                PhoneNumberConfirmed = false,
                SecurityStamp = Guid.NewGuid().ToString()
            };

            var result = await _userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                _logger.LogInformation("✅ User created successfully: {Email} with ID: {Id}", normalizedEmail, user.Id);

                // Test immédiat du mot de passe
                var passwordTest = await _userManager.CheckPasswordAsync(user, password);
                _logger.LogInformation("🔍 Immediate password test: {IsValid}", passwordTest);

                return AuthResult.Success(
                    token: "registration-success", // Pas de token à la création
                    expiry: DateTime.UtcNow.AddDays(1),
                    email: user.Email!,
                    userName: user.UserName,
                    nom: user.Nom,
                    prenom: user.Prenom,
                    userId: user.Id
                );
            }
            else
            {
                var errors = result.Errors.Select(e => $"{e.Code}: {e.Description}").ToArray();
                _logger.LogError("❌ User creation failed: {Errors}", string.Join(", ", errors));
                return AuthResult.Failure(errors);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error during registration for: {Email}", email);
            return AuthResult.Failure($"Une erreur s'est produite: {ex.Message}");
        }
    }

    public async Task<bool> UserExistsAsync(string email)
    {
        try
        {
            var normalizedEmail = email.ToLower().Trim();
            var user = await _userManager.FindByEmailAsync(normalizedEmail)
                      ?? await _userManager.FindByNameAsync(normalizedEmail);
            return user != null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error checking if user exists: {Email}", email);
            return false;
        }
    }
}