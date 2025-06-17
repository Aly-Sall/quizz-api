// src/Application/Account/Commands/VerifyCandidateAccess/VerifyCandidateAccessHandler.cs - VERSION AVEC ALIAS
using _Net6CleanArchitectureQuizzApp.Application.Account.Commands.VerifyCandidateAccess;
using _Net6CleanArchitectureQuizzApp.Application.Account.Models;
using _Net6CleanArchitectureQuizzApp.Application.Common.Interfaces;
using _Net6CleanArchitectureQuizzApp.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;
// Créer un alias pour éviter la confusion
using TestDtoModel = _Net6CleanArchitectureQuizzApp.Application.Account.Models.TestDto;

namespace _Net6CleanArchitectureQuizzApp.Application.Account.Commands.VerifyCandidateAccess;

public class VerifyCandidateAccessHandler : IRequestHandler<VerifyCandidateAccessCommand, AuthResponseExtended>
{
    private readonly IAuthService _authService;
    private readonly ILogger<VerifyCandidateAccessHandler> _logger;

    public VerifyCandidateAccessHandler(
        IAuthService authService,
        ILogger<VerifyCandidateAccessHandler> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    public async Task<AuthResponseExtended> Handle(VerifyCandidateAccessCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("🔍 Verifying candidate access with token: {Token}", request.Token.Substring(0, Math.Min(8, request.Token.Length)) + "...");

        var result = await _authService.VerifyCandidateAccessAsync(request.Token);

        if (result.IsSuccess)
        {
            _logger.LogInformation("✅ Candidate access verified for {Email}", result.Email);

            return new AuthResponseExtended
            {
                Token = result.AuthToken,
                Expiry = result.Expiry,
                UserRole = UserRole.Candidate,
                Email = result.Email,
                Nom = result.Nom,
                Prenom = result.Prenom,
                RequiresEmailInvitation = false,
                // Utiliser l'alias pour clarifier quel TestDto on utilise
                AvailableTests = result.AvailableTests ?? new List<TestDtoModel>()
            };
        }

        _logger.LogWarning("❌ Candidate access verification failed: {Error}", result.ErrorMessage);
        throw new UnauthorizedAccessException(result.ErrorMessage ?? "Token invalide");
    }
}