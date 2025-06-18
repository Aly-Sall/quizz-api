// src/Application/Account/Commands/VerifyCandidateAccess/VerifyCandidateAccessHandler.cs - VERSION CORRIGÉE
using _Net6CleanArchitectureQuizzApp.Application.Account.Models;
using _Net6CleanArchitectureQuizzApp.Application.Common.Interfaces;
using _Net6CleanArchitectureQuizzApp.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;

namespace _Net6CleanArchitectureQuizzApp.Application.Account.Commands.VerifyCandidateAccess;

// ✅ COMMANDE AVEC UN NOM DIFFÉRENT POUR ÉVITER LE CONFLIT
public class CandidateTokenVerificationCommand : IRequest<AuthResponseExtended>
{
    [Required(ErrorMessage = "Le token d'accès est requis")]
    public string Token { get; set; } = string.Empty;

    public CandidateTokenVerificationCommand() { }
    public CandidateTokenVerificationCommand(string token) => Token = token;
}

public class VerifyCandidateAccessHandler : IRequestHandler<CandidateTokenVerificationCommand, AuthResponseExtended>
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

    public async Task<AuthResponseExtended> Handle(CandidateTokenVerificationCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("🔍 Verifying candidate access with token: {Token}...",
            request.Token.Substring(0, Math.Min(8, request.Token.Length)));

        try
        {
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
                    // IsSuccess = true par défaut, pas besoin de le définir
                    AvailableTests = result.AvailableTests ?? new List<TestDto>()
                };
            }

            _logger.LogWarning("❌ Candidate access verification failed: {Error}", result.ErrorMessage);

            return new AuthResponseExtended
            {
                IsSuccess = false, // Explicitement false pour l'échec
                Message = result.ErrorMessage ?? "Token invalide",
                UserRole = UserRole.Candidate,
                AvailableTests = new List<TestDto>()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error during candidate access verification");

            return new AuthResponseExtended
            {
                IsSuccess = false, // Explicitement false pour l'erreur
                Message = "Erreur lors de la vérification de l'accès",
                UserRole = UserRole.Candidate,
                AvailableTests = new List<TestDto>()
            };
        }
    }
}