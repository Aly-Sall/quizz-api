// src/Infrastructure/Services/AuthService.cs - REMPLACER LE CONTENU EXISTANT
using _Net6CleanArchitectureQuizzApp.Application.Common.Interfaces;
using _Net6CleanArchitectureQuizzApp.Application.Common.Models;
using _Net6CleanArchitectureQuizzApp.Domain.Entities;
using _Net6CleanArchitectureQuizzApp.Domain.Enums;
using _Net6CleanArchitectureQuizzApp.Domain.Interfaces;// CORRECTION: Import depuis Domain
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
// Alias pour éviter les conflits de noms
using TestDtoModel = _Net6CleanArchitectureQuizzApp.Application.Account.Models.TestDto;

namespace _Net6CleanArchitectureQuizzApp.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly IApplicationDbContext _context;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        IApplicationDbContext context,
        IJwtTokenGenerator jwtTokenGenerator,
        ILogger<AuthService> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _context = context;
        _jwtTokenGenerator = jwtTokenGenerator;
        _logger = logger;
    }

    // MÉTHODES EXISTANTES (mises à jour)
    public async Task<AuthResult> LoginAsync(string email, string password)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(email.Trim().ToLower());
            if (user == null)
            {
                return AuthResult.Failure("Utilisateur non trouvé");
            }

            var isPasswordValid = await _userManager.CheckPasswordAsync(user, password);
            if (!isPasswordValid)
            {
                return AuthResult.Failure("Mot de passe incorrect");
            }

            // Générer le token JWT en utilisant la méthode existante
            var tokenResult = _jwtTokenGenerator.GenerateToken(user);

            // Mettre à jour la dernière connexion
            user.LastLoginDate = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);

            return AuthResult.Success(tokenResult.Token, tokenResult.Expiry, user.Email!, user.UserName, user.Nom, user.Prenom, user.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la connexion pour {Email}", email);
            return AuthResult.Failure("Une erreur s'est produite lors de la connexion");
        }
    }

    public async Task<AuthResult> RegisterAsync(string email, string password, string? nom = null, string? prenom = null)
    {
        try
        {
            var user = new User
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true,
                Nom = nom ?? "",
                Prenom = prenom ?? "",
                UserRole = UserRole.Administrator // Par défaut Administrator
            };

            var result = await _userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                return AuthResult.Success("", DateTime.UtcNow, user.Email!, user.UserName, user.Nom, user.Prenom, user.Id);
            }

            var errors = result.Errors.Select(e => e.Description).ToArray();
            return AuthResult.Failure(errors);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de l'inscription pour {Email}", email);
            return AuthResult.Failure("Une erreur s'est produite lors de l'inscription");
        }
    }

    public async Task<bool> UserExistsAsync(string email)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(email.Trim().ToLower());
            return user != null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la vérification d'existence pour {Email}", email);
            return false;
        }
    }

    // NOUVELLES MÉTHODES POUR LA GESTION DES RÔLES
    public async Task<AuthResultWithRole> LoginWithRoleAsync(string email, string password, UserRole expectedRole)
    {
        try
        {
            _logger.LogInformation("🔍 LoginWithRole attempt for {Email} with role {Role}", email, expectedRole);

            var user = await _userManager.FindByEmailAsync(email.Trim().ToLower());
            if (user == null)
            {
                _logger.LogWarning("❌ User not found: {Email}", email);
                return AuthResultWithRole.Failure("Utilisateur non trouvé");
            }

            // Vérifier le rôle
            if (user.UserRole != expectedRole)
            {
                _logger.LogWarning("❌ Role mismatch for {Email}. Expected: {Expected}, Actual: {Actual}",
                    email, expectedRole, user.UserRole);
                return AuthResultWithRole.Failure($"Cet utilisateur n'a pas le rôle {expectedRole}");
            }

            // Vérifier le mot de passe
            var isPasswordValid = await _userManager.CheckPasswordAsync(user, password);
            if (!isPasswordValid)
            {
                _logger.LogWarning("❌ Invalid password for {Email}", email);
                return AuthResultWithRole.Failure("Mot de passe incorrect");
            }

            // Générer le token si c'est un admin
            string? token = null;
            DateTime? expiry = null;

            if (expectedRole == UserRole.Administrator)
            {
                var tokenResult = _jwtTokenGenerator.GenerateToken(user);
                token = tokenResult.Token;
                expiry = tokenResult.Expiry;
            }

            // Mettre à jour la dernière connexion
            user.LastLoginDate = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);

            _logger.LogInformation("✅ LoginWithRole successful for {Email}", email);

            return AuthResultWithRole.Success(
                token: token,
                expiry: expiry,
                email: user.Email!,
                nom: user.Nom,
                prenom: user.Prenom,
                userRole: user.UserRole
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error during LoginWithRole for {Email}", email);
            return AuthResultWithRole.Failure("Une erreur s'est produite lors de la connexion");
        }
    }

    public async Task<CandidateAccessResult> VerifyCandidateAccessAsync(string accessToken)
    {
        try
        {
            _logger.LogInformation("🔍 Verifying candidate access token: {Token}", accessToken.Substring(0, Math.Min(8, accessToken.Length)) + "...");

            var candidateAccess = await _context.CandidateAccessTokens
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.Token == accessToken &&
                                   c.ExpirationTime > DateTime.UtcNow &&
                                   !c.IsUsed);

            if (candidateAccess == null)
            {
                _logger.LogWarning("❌ Invalid or expired candidate access token");
                return CandidateAccessResult.Failure("Token d'accès invalide ou expiré");
            }

            // Marquer le token comme utilisé
            candidateAccess.IsUsed = true;
            await _context.SaveChangesAsync();

            // Générer un token JWT pour la session en utilisant la méthode existante
            var tokenResult = _jwtTokenGenerator.GenerateToken(candidateAccess.User);

            // Récupérer les tests disponibles pour ce candidat
            var availableTests = await GetAvailableTestsForCandidateAsync(candidateAccess.User.Email!);

            _logger.LogInformation("✅ Candidate access verified for {Email}", candidateAccess.User.Email);

            return CandidateAccessResult.Success(
                authToken: tokenResult.Token,
                expiry: DateTime.UtcNow.AddHours(4), // Session plus courte pour les candidats
                email: candidateAccess.User.Email!,
                nom: candidateAccess.User.Nom,
                prenom: candidateAccess.User.Prenom,
                availableTests: availableTests
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error during candidate access verification");
            return CandidateAccessResult.Failure("Une erreur s'est produite lors de la vérification");
        }
    }

    // MÉTHODE UTILITAIRE PRIVÉE
    private async Task<List<TestDtoModel>> GetAvailableTestsForCandidateAsync(string candidateEmail)
    {
        try
        {
            return await _context.TestAccessTokens
                .Where(t => t.CandidateEmail == candidateEmail &&
                           t.ExpirationTime > DateTime.UtcNow &&
                           !t.IsUsed)
                .Join(_context.Tests,
                      token => token.TestId,
                      test => test.Id,
                      (token, test) => new TestDtoModel
                      {
                          Id = test.Id,
                          Title = test.Title ?? "Test sans titre",
                          Description = "", // CORRECTION: QuizTest n'a pas de Description
                          Duration = test.Duration
                      })
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error retrieving available tests for {Email}", candidateEmail);
            return new List<TestDtoModel>();
        }
    }
}