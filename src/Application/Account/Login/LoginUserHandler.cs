// src/Application/Account/Commands/Login/LoginHandler.cs
using System;
using System.Threading;
using System.Threading.Tasks;
using _Net6CleanArchitectureQuizzApp.Application.Common.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace _Net6CleanArchitectureQuizzApp.Application.Account.Commands.Login;

public class LoginHandler : IRequestHandler<LoginModel, AuthResponse>
{
    private readonly IAuthService _authService;
    private readonly ILogger<LoginHandler> _logger;

    public LoginHandler(
        IAuthService authService,
        ILogger<LoginHandler> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    public async Task<AuthResponse> Handle(LoginModel request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("🔍 LOGIN ATTEMPT - Email: {Email}", request.Email);

        try
        {
            // ✅ Validation de base
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            {
                _logger.LogWarning("❌ Email or password is empty");
                throw new UnauthorizedAccessException("Email et mot de passe requis");
            }

            // ✅ Déléguer l'authentification au service d'infrastructure
            var result = await _authService.LoginAsync(request.Email.Trim(), request.Password);

            if (result.IsSuccess)
            {
                _logger.LogInformation("✅ Login successful for user: {Email}", request.Email);

                return new AuthResponse(
                    Token: result.Token!,
                    Expiry: result.Expiry!.Value,
                    Nom: result.Nom,
                    Prenom: result.Prenom,
                    UserName: result.UserName,
                    Email: result.Email
                );
            }
            else
            {
                _logger.LogWarning("❌ Login failed for user: {Email} - {Error}", request.Email, result.ErrorMessage);
                throw new UnauthorizedAccessException(result.ErrorMessage ?? "Email ou mot de passe incorrect");
            }
        }
        catch (UnauthorizedAccessException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Unexpected error during login for: {Email}", request.Email);
            throw new UnauthorizedAccessException("Une erreur s'est produite lors de la connexion");
        }
    }
}