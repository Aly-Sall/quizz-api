// src/Application/Account/Register/RegisterUserHandler.cs
using System;
using System.Threading;
using System.Threading.Tasks;
using _Net6CleanArchitectureQuizzApp.Application.Common.Interfaces;
using _Net6CleanArchitectureQuizzApp.Application.Common.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace _Net6CleanArchitectureQuizzApp.Application.Account.Commands.Register;

public class RegisterUserHandler : IRequestHandler<RegisterUserModel, Result>
{
    private readonly IAuthService _authService;
    private readonly ILogger<RegisterUserHandler> _logger;

    public RegisterUserHandler(
        IAuthService authService,
        ILogger<RegisterUserHandler> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    public async Task<Result> Handle(RegisterUserModel request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("🔍 REGISTRATION ATTEMPT - Email: {Email}", request.Email);

        try
        {
            // ✅ Validation de base
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

            var email = request.Email.Trim();
            _logger.LogInformation("🔍 Processing registration for: {Email}", email);

            // ✅ Vérifier si l'utilisateur existe déjà
            var userExists = await _authService.UserExistsAsync(email);
            if (userExists)
            {
                _logger.LogWarning("❌ User already exists with email: {Email}", email);
                return Result.Failure("Un utilisateur avec cet email existe déjà");
            }

            // ✅ Déléguer l'enregistrement au service d'infrastructure
            var result = await _authService.RegisterAsync(email, request.Password, request.Nom, request.Prenom);

            if (result.IsSuccess)
            {
                _logger.LogInformation("✅ User registered successfully: {Email} with ID: {Id}", email, result.UserId);
                return Result.Success(result.UserId);
            }
            else
            {
                _logger.LogError("❌ Registration failed for {Email}: {Errors}",
                    email, result.ErrorMessage ?? string.Join(", ", result.Errors ?? new string[0]));

                return Result.Failure(result.ErrorMessage ?? string.Join(", ", result.Errors ?? new string[0]));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Unexpected error during registration");
            return Result.Failure($"Une erreur inattendue s'est produite: {ex.Message}");
        }
    }
}