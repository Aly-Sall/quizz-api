// src/Application/Account/Commands/Login/LoginHandler.cs - VERSION AVEC GESTION DES RÔLES
using _Net6CleanArchitectureQuizzApp.Application.Common.Interfaces;
using _Net6CleanArchitectureQuizzApp.Application.Account.Models;
using _Net6CleanArchitectureQuizzApp.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace _Net6CleanArchitectureQuizzApp.Application.Account.Commands.Login;

public class LoginHandler : IRequestHandler<LoginModel, AuthResponseExtended>
{
    private readonly IAuthService _authService;
    private readonly ICandidateInvitationService _candidateInvitationService;
    private readonly ILogger<LoginHandler> _logger;

    public LoginHandler(
        IAuthService authService,
        ICandidateInvitationService candidateInvitationService,
        ILogger<LoginHandler> logger)
    {
        _authService = authService;
        _candidateInvitationService = candidateInvitationService;
        _logger = logger;
    }

    public async Task<AuthResponseExtended> Handle(LoginModel request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("🔍 Login attempt with role {Role} for {Email}", request.UserRole, request.Email);

            var result = await _authService.LoginWithRoleAsync(request.Email, request.Password, request.UserRole);

            if (result.IsSuccess)
            {
                if (request.UserRole == UserRole.Administrator)
                {
                    _logger.LogInformation("✅ Admin login successful for {Email}", request.Email);

                    // Admin - retourner directement le token
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
                    _logger.LogInformation("✅ Candidate login successful for {Email} - sending invitation email", request.Email);

                    // Candidat - envoyer email d'invitation
                    var emailSent = await _candidateInvitationService.SendCandidateInvitationEmailAsync(
                        result.Email!,
                        $"{result.Prenom} {result.Nom}");

                    return new AuthResponseExtended
                    {
                        Message = emailSent ?
                            "Un email d'invitation vous a été envoyé. Veuillez vérifier votre boîte mail." :
                            "Erreur lors de l'envoi de l'email",
                        Email = result.Email,
                        Nom = result.Nom,
                        Prenom = result.Prenom,
                        RequiresEmailInvitation = true,
                        UserRole = result.UserRole,
                        // ✅ PAS DE TOKEN POUR LES CANDIDATS
                        Token = null,
                        Expiry = null
                    };
                }
            }

            _logger.LogWarning("❌ Login failed for {Email}: {Error}", request.Email, result.ErrorMessage);

            return new AuthResponseExtended
            {
                IsSuccess = false,
                Message = result.ErrorMessage ?? "Email ou mot de passe incorrect",
                UserRole = request.UserRole
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error during login for {Email}", request.Email);

            return new AuthResponseExtended
            {
                IsSuccess = false,
                Message = "Une erreur s'est produite lors de la connexion",
                UserRole = request.UserRole
            };
        }
    }
}