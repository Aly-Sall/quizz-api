// src/Application/Account/Commands/LoginWithRole/LoginWithRoleHandler.cs - NOUVEAU FICHIER
using _Net6CleanArchitectureQuizzApp.Application.Account.Commands.LoginWithRole;
using _Net6CleanArchitectureQuizzApp.Application.Account.Models; // Import pour AuthResponseExtended
using _Net6CleanArchitectureQuizzApp.Application.Common.Interfaces;
using _Net6CleanArchitectureQuizzApp.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace _Net6CleanArchitectureQuizzApp.Application.Account.Commands.LoginWithRole;

public class LoginWithRoleHandler : IRequestHandler<LoginWithRoleCommand, AuthResponseExtended>
{
    private readonly IAuthService _authService;
    private readonly ICandidateInvitationService _candidateInvitationService;
    private readonly ILogger<LoginWithRoleHandler> _logger;

    public LoginWithRoleHandler(
        IAuthService authService,
        ICandidateInvitationService candidateInvitationService,
        ILogger<LoginWithRoleHandler> logger)
    {
        _authService = authService;
        _candidateInvitationService = candidateInvitationService;
        _logger = logger;
    }

    public async Task<AuthResponseExtended> Handle(LoginWithRoleCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("🔍 Login attempt with role {Role} for {Email}", request.UserRole, request.Email);

        var result = await _authService.LoginWithRoleAsync(request.Email, request.Password, request.UserRole);

        if (result.IsSuccess)
        {
            if (request.UserRole == UserRole.Administrator)
            {
                return new AuthResponseExtended
                {
                    Token = result.Token,
                    Expiry = result.Expiry,
                    UserRole = result.UserRole,
                    Email = result.Email,
                    Nom = result.Nom,
                    Prenom = result.Prenom,
                    RequiresEmailInvitation = false
                };
            }
            else if (request.UserRole == UserRole.Candidate)
            {
                var emailSent = await _candidateInvitationService.SendCandidateInvitationEmailAsync(
                    result.Email!,
                    $"{result.Prenom} {result.Nom}");

                return new AuthResponseExtended
                {
                    Message = emailSent ? "Un email d'invitation vous a été envoyé." : "Erreur lors de l'envoi de l'email",
                    Email = result.Email,
                    RequiresEmailInvitation = true,
                    UserRole = result.UserRole
                };
            }
        }

        throw new UnauthorizedAccessException(result.ErrorMessage ?? "Connexion échouée");
    }
}