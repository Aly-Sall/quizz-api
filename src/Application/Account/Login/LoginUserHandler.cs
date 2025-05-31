using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _Net6CleanArchitectureQuizzApp.Domain.Entities;
using _Net6CleanArchitectureQuizzApp.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace _Net6CleanArchitectureQuizzApp.Application.Account.Commands.Login;

public class LoginHandler : IRequestHandler<LoginModel, AuthResponse>
{
    private readonly UserManager<User> _userManager;
    private readonly IJwtTokenGenerator _tokenGenerator;
    private readonly ILogger<LoginHandler> _logger;

    public LoginHandler(
        UserManager<User> userManager,
        IJwtTokenGenerator tokenGenerator,
        ILogger<LoginHandler> logger)
    {
        _userManager = userManager;
        _tokenGenerator = tokenGenerator;
        _logger = logger;
    }

    public async Task<AuthResponse> Handle(LoginModel request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("🔍 LOGIN ATTEMPT - Email: {Email}", request.Email);
        _logger.LogInformation("🔍 Password length: {Length}", request.Password?.Length ?? 0);

        // ✅ Chercher par email ET par username
        var user = await _userManager.FindByEmailAsync(request.Email);
        _logger.LogInformation("🔍 User found by email: {Found}", user != null);

        if (user == null)
        {
            user = await _userManager.FindByNameAsync(request.Email);
            _logger.LogInformation("🔍 User found by username: {Found}", user != null);
        }

        if (user == null)
        {
            _logger.LogWarning("❌ User not found for email: {Email}", request.Email);
            throw new UnauthorizedAccessException("Email ou mot de passe incorrect");
        }

        _logger.LogInformation("🔍 User details - ID: {Id}, Email: {Email}, UserName: {UserName}",
            user.Id, user.Email, user.UserName);
        _logger.LogInformation("🔍 EmailConfirmed: {EmailConfirmed}, LockoutEnabled: {LockoutEnabled}",
            user.EmailConfirmed, user.LockoutEnabled);

        // ✅ Vérifier le verrouillage avant la validation du mot de passe
        if (await _userManager.IsLockedOutAsync(user))
        {
            _logger.LogWarning("❌ Account locked out for user: {Email}", request.Email);
            throw new UnauthorizedAccessException("Compte temporairement bloqué");
        }

        // ✅ Validation du mot de passe avec logs détaillés
        var isPasswordValid = await _userManager.CheckPasswordAsync(user, request.Password);
        _logger.LogInformation("🔍 Password valid: {Valid}", isPasswordValid);

        if (!isPasswordValid)
        {
            _logger.LogWarning("❌ Invalid password for user: {Email}", request.Email);

            // ✅ Debug du hash de mot de passe
            var hasPasswordHash = !string.IsNullOrEmpty(user.PasswordHash);
            _logger.LogInformation("🔍 User has password hash: {HasHash}", hasPasswordHash);

            if (hasPasswordHash)
            {
                _logger.LogInformation("🔍 Password hash length: {Length}", user.PasswordHash?.Length ?? 0);
            }

            // ✅ Incrémenter les tentatives échouées
            await _userManager.AccessFailedAsync(user);

            throw new UnauthorizedAccessException("Email ou mot de passe incorrect");
        }

        // ✅ Réinitialiser le compteur d'échecs en cas de succès
        await _userManager.ResetAccessFailedCountAsync(user);

        // ✅ Générer le token
        var (token, expiry) = _tokenGenerator.GenerateToken(user);

        _logger.LogInformation("✅ Login successful for user: {Email}", request.Email);

        return new AuthResponse(
            Token: token,
            Expiry: expiry,
            Nom: user.Nom,
            Prenom: user.Prenom,
            UserName: user.UserName,
            Email: user.Email
        );
    }
}
