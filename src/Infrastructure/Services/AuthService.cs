// src/Infrastructure/Services/AuthService.cs - CORRIGÉ POUR CANDIDATS
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using _Net6CleanArchitectureQuizzApp.Application.Common.Interfaces;
using _Net6CleanArchitectureQuizzApp.Domain.Entities;
using _Net6CleanArchitectureQuizzApp.Domain.Enums;

namespace _Net6CleanArchitectureQuizzApp.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        IJwtTokenGenerator jwtTokenGenerator,
        ILogger<AuthService> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtTokenGenerator = jwtTokenGenerator;
        _logger = logger;
    }

    public async Task<AuthResult> LoginAsync(string email, string password)
    {
        try
        {
            _logger.LogInformation("🔍 Login attempt for: {Email}", email);

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                _logger.LogWarning("❌ User not found: {Email}", email);
                return AuthResult.Failure("Email ou mot de passe incorrect");
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, password, false);
            if (result.Succeeded)
            {
                // CORRIGÉ : Utiliser la signature correcte qui retourne un tuple
                var tokenResult = _jwtTokenGenerator.GenerateToken(user);

                _logger.LogInformation("✅ Login successful for: {Email}", email);

                return AuthResult.Success(
                    token: tokenResult.Token,
                    expiry: tokenResult.Expiry,
                    email: user.Email!,
                    userName: user.UserName,
                    nom: user.Nom,
                    prenom: user.Prenom,
                    userId: user.Id
                );
            }

            _logger.LogWarning("❌ Invalid password for: {Email}", email);
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
            _logger.LogInformation("🔍 Registration attempt for: {Email}", email);

            // Vérifier si l'utilisateur existe déjà
            var existingUser = await _userManager.FindByEmailAsync(email);
            if (existingUser != null)
            {
                _logger.LogWarning("❌ User already exists: {Email}", email);
                return AuthResult.Failure("Un utilisateur avec cet email existe déjà");
            }

            // Créer le nouvel utilisateur - UTILISER SEULEMENT LES CHAMPS QUI EXISTENT
            var user = new User
            {
                UserName = email,
                Email = email,
                Nom = nom ?? "",
                Prenom = prenom ?? "",
                EmailConfirmed = true // Simplifier pour les tests
            };

            var result = await _userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                _logger.LogInformation("✅ User registered successfully: {Email}", email);

                // CORRIGÉ : Utiliser la signature correcte qui retourne un tuple
                var tokenResult = _jwtTokenGenerator.GenerateToken(user);

                return AuthResult.Success(
                    token: tokenResult.Token,
                    expiry: tokenResult.Expiry,
                    email: user.Email,
                    userName: user.UserName,
                    nom: user.Nom,
                    prenom: user.Prenom,
                    userId: user.Id
                );
            }
            else
            {
                var errors = result.Errors.Select(e => e.Description).ToArray();
                _logger.LogWarning("❌ Registration failed for {Email}: {Errors}", email, string.Join(", ", errors));
                return AuthResult.Failure(errors);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error during registration for: {Email}", email);
            return AuthResult.Failure("Une erreur s'est produite lors de l'inscription");
        }
    }

    public async Task<bool> UserExistsAsync(string email)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(email);
            return user != null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error checking if user exists: {Email}", email);
            return false;
        }
    }

    // ✅ MÉTHODE CORRIGÉE POUR GÉRER CORRECTEMENT ADMIN VS CANDIDAT
    public async Task<AuthResultWithRole> LoginWithRoleAsync(string email, string password, UserRole expectedRole)
    {
        try
        {
            _logger.LogInformation("🔍 Login with role attempt for {Email} as {Role}", email, expectedRole);

            // Vérifier d'abord si l'utilisateur existe et si le mot de passe est correct
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                _logger.LogWarning("❌ User not found: {Email}", email);
                return AuthResultWithRole.Failure("Email ou mot de passe incorrect");
            }

            var passwordValid = await _userManager.CheckPasswordAsync(user, password);
            if (!passwordValid)
            {
                _logger.LogWarning("❌ Invalid password for user: {Email}", email);
                return AuthResultWithRole.Failure("Email ou mot de passe incorrect");
            }

            _logger.LogInformation("✅ User credentials validated for {Email}", email);

            // ✅ Pour les admins, générer et retourner un token JWT
            if (expectedRole == UserRole.Administrator)
            {
                var tokenResult = _jwtTokenGenerator.GenerateToken(user);

                _logger.LogInformation("✅ Admin token generated for {Email}", email);

                return AuthResultWithRole.Success(
                    tokenResult.Token,
                    tokenResult.Expiry,
                    user.Email!,
                    user.Nom ?? "",
                    user.Prenom ?? "",
                    UserRole.Administrator
                );
            }

            // ✅ Pour les candidats, NE PAS générer de token JWT
            // Retourner juste les infos utilisateur sans token
            if (expectedRole == UserRole.Candidate)
            {
                _logger.LogInformation("✅ Candidate credentials validated for {Email} - no token generated", email);

                return AuthResultWithRole.SuccessWithoutToken(
                    user.Email!,
                    user.Nom ?? "",
                    user.Prenom ?? "",
                    UserRole.Candidate
                );
            }

            return AuthResultWithRole.Failure("Rôle non supporté");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error during login with role for {Email}", email);
            return AuthResultWithRole.Failure("Une erreur s'est produite lors de la connexion");
        }
    }

    public async Task<CandidateAccessResult> VerifyCandidateAccessAsync(string accessToken)
    {
        // Méthode temporaire - à implémenter plus tard
        await Task.Delay(1);
        return CandidateAccessResult.Failure("Fonctionnalité candidat pas encore implémentée");
    }
}