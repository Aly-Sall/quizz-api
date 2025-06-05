using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using _Net6CleanArchitectureQuizzApp.Application.Common.Interfaces;
using _Net6CleanArchitectureQuizzApp.Application.Common.Models;
using _Net6CleanArchitectureQuizzApp.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace _Net6CleanArchitectureQuizzApp.Application.Account.Commands.Register
{
    public class RegisterUserHandler : IRequestHandler<RegisterUserModel, Result>
    {
        private readonly UserManager<User> _userManager;
        private readonly ILogger<RegisterUserHandler> _logger;

        public RegisterUserHandler(
            UserManager<User> userManager,
            ILogger<RegisterUserHandler> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<Result> Handle(RegisterUserModel request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("🔍 REGISTRATION ATTEMPT - Email: {Email}", request.Email);

            try
            {
                // ✅ Validation de base renforcée
                if (string.IsNullOrWhiteSpace(request.Email))
                {
                    return Result.Failure("L'email est requis");
                }

                if (string.IsNullOrWhiteSpace(request.Password))
                {
                    return Result.Failure("Le mot de passe est requis");
                }

                if (request.Password.Length < 6)
                {
                    return Result.Failure("Le mot de passe doit contenir au moins 6 caractères");
                }

                var email = request.Email.Trim().ToLower();
                _logger.LogInformation("🔍 Processing registration for: {Email}", email);

                // ✅ Vérifier les options de mot de passe configurées
                var passwordOptions = _userManager.Options.Password;
                _logger.LogInformation("🔍 Password options - RequiredLength: {Length}, RequireDigit: {Digit}, RequireUpper: {Upper}",
                    passwordOptions.RequiredLength, passwordOptions.RequireDigit, passwordOptions.RequireUppercase);

                // ✅ Vérifier si l'utilisateur existe déjà (par email ET username)
                var existingUserByEmail = await _userManager.FindByEmailAsync(email);
                if (existingUserByEmail != null)
                {
                    _logger.LogWarning("❌ User already exists with email: {Email}", email);
                    return Result.Failure("Un utilisateur avec cet email existe déjà");
                }

                var existingUserByUsername = await _userManager.FindByNameAsync(email);
                if (existingUserByUsername != null)
                {
                    _logger.LogWarning("❌ User already exists with username: {Email}", email);
                    return Result.Failure("Un utilisateur avec ce nom d'utilisateur existe déjà");
                }

                // ✅ Créer le nouvel utilisateur avec normalisation explicite
                var user = new User
                {
                    Email = email,
                    UserName = email,
                    NormalizedEmail = email.ToUpperInvariant(),
                    NormalizedUserName = email.ToUpperInvariant(),
                    Nom = request.Nom?.Trim(),
                    Prenom = request.Prenom?.Trim(),
                    EmailConfirmed = true,
                    LockoutEnabled = false,
                    AccessFailedCount = 0,
                    TwoFactorEnabled = false,
                    PhoneNumberConfirmed = false,
                    SecurityStamp = Guid.NewGuid().ToString()
                };

                _logger.LogInformation("🔍 Creating user with:");
                _logger.LogInformation("  Email: {Email}", user.Email);
                _logger.LogInformation("  UserName: {UserName}", user.UserName);
                _logger.LogInformation("  NormalizedEmail: {NormalizedEmail}", user.NormalizedEmail);
                _logger.LogInformation("  Password length: {Length}", request.Password.Length);

                // ✅ Créer l'utilisateur avec UserManager
                var result = await _userManager.CreateAsync(user, request.Password);

                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select(e => $"{e.Code}: {e.Description}").ToArray();
                    var errorMessage = string.Join(" | ", errors);

                    _logger.LogError("❌ User creation failed: {Errors}", errorMessage);

                    // Log détaillé de chaque erreur
                    foreach (var error in result.Errors)
                    {
                        _logger.LogError("❌ Identity Error - Code: {Code}, Description: {Description}",
                            error.Code, error.Description);
                    }

                    return Result.Failure($"Échec de création: {errorMessage}");
                }

                // ✅ Récupérer l'utilisateur créé pour vérifications
                var createdUser = await _userManager.FindByEmailAsync(email);
                if (createdUser == null)
                {
                    _logger.LogError("❌ Could not find user after creation");
                    return Result.Failure("Erreur lors de la récupération de l'utilisateur créé");
                }

                _logger.LogInformation("✅ User created successfully with ID: {Id}", createdUser.Id);
                _logger.LogInformation("🔍 Created user details:");
                _logger.LogInformation("  ID: {Id}", createdUser.Id);
                _logger.LogInformation("  Email: {Email}", createdUser.Email);
                _logger.LogInformation("  UserName: {UserName}", createdUser.UserName);
                _logger.LogInformation("  Has PasswordHash: {HasHash}", !string.IsNullOrEmpty(createdUser.PasswordHash));
                _logger.LogInformation("  PasswordHash length: {Length}", createdUser.PasswordHash?.Length ?? 0);
                _logger.LogInformation("  EmailConfirmed: {EmailConfirmed}", createdUser.EmailConfirmed);
                _logger.LogInformation("  LockoutEnabled: {LockoutEnabled}", createdUser.LockoutEnabled);

                // ✅ Test IMMÉDIAT du mot de passe pour s'assurer qu'il fonctionne
                _logger.LogInformation("🔍 Testing password immediately after creation...");
                var immediatePasswordTest = await _userManager.CheckPasswordAsync(createdUser, request.Password);
                _logger.LogInformation("🔍 Immediate password test result: {IsValid}", immediatePasswordTest);

                if (!immediatePasswordTest)
                {
                    _logger.LogError("⚠️ CRITICAL: Password verification failed immediately after creation!");
                    _logger.LogError("🔍 This indicates a problem with Identity configuration");

                    // Essayer avec le PasswordHasher directement
                    var hasher = new PasswordHasher<User>();
                    var manualVerification = hasher.VerifyHashedPassword(createdUser, createdUser.PasswordHash, request.Password);
                    _logger.LogError("🔍 Manual PasswordHasher result: {Result}", manualVerification);

                    // Ne pas retourner d'erreur pour l'instant, juste loguer
                    _logger.LogWarning("⚠️ Continuing despite password test failure - check Identity configuration");
                }
                else
                {
                    _logger.LogInformation("✅ Password verification successful immediately after creation");
                }

                // ✅ Test de recherche de l'utilisateur
                var findTest1 = await _userManager.FindByEmailAsync(email);
                var findTest2 = await _userManager.FindByNameAsync(email);
                _logger.LogInformation("🔍 Find by email test: {Found}", findTest1 != null);
                _logger.LogInformation("🔍 Find by username test: {Found}", findTest2 != null);

                return Result.Success(createdUser.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Unexpected error during registration");
                return Result.Failure($"Une erreur inattendue s'est produite: {ex.Message}");
            }
        }
    }
}