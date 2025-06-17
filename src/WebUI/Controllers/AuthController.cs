// src/WebUI/Controllers/AuthController.cs - Version simplifiée fonctionnelle
using _Net6CleanArchitectureQuizzApp.Application.Common.Interfaces;
using _Net6CleanArchitectureQuizzApp.Application.Account.Models;
using _Net6CleanArchitectureQuizzApp.Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace _Net6CleanArchitectureQuizzApp.WebUI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ICandidateInvitationService _candidateInvitationService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        IAuthService authService,
        ICandidateInvitationService candidateInvitationService,
        ILogger<AuthController> logger)
    {
        _authService = authService;
        _candidateInvitationService = candidateInvitationService;
        _logger = logger;
    }

    [HttpPost("login-with-role")]
    public async Task<IActionResult> LoginWithRole([FromBody] LoginWithRoleRequest request)
    {
        try
        {
            _logger.LogInformation("🔍 Login attempt with role {Role} for {Email}", request.UserRole, request.Email);

            var result = await _authService.LoginWithRoleAsync(request.Email, request.Password, request.UserRole);

            if (result.IsSuccess)
            {
                if (request.UserRole == UserRole.Administrator)
                {
                    // Admin - retourner directement le token
                    return Ok(new AuthResponseExtended
                    {
                        Token = result.Token,
                        Expiry = result.Expiry,
                        UserRole = result.UserRole,
                        Email = result.Email,
                        Nom = result.Nom,
                        Prenom = result.Prenom,
                        RequiresEmailInvitation = false
                    });
                }
                else if (request.UserRole == UserRole.Candidate)
                {
                    // Candidat - envoyer email d'invitation
                    var emailSent = await _candidateInvitationService.SendCandidateInvitationEmailAsync(
                        result.Email!,
                        $"{result.Prenom} {result.Nom}");

                    return Ok(new AuthResponseExtended
                    {
                        Message = emailSent ?
                            "Un email d'invitation vous a été envoyé. Veuillez vérifier votre boîte mail." :
                            "Erreur lors de l'envoi de l'email",
                        Email = result.Email,
                        RequiresEmailInvitation = true,
                        UserRole = result.UserRole
                    });
                }
            }

            return BadRequest(new { error = result.ErrorMessage });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la connexion");
            return StatusCode(500, new { error = "Une erreur s'est produite" });
        }
    }

    [HttpPost("verify-candidate-access")]
    public async Task<IActionResult> VerifyCandidateAccess([FromBody] VerifyCandidateAccessRequest request)
    {
        try
        {
            _logger.LogInformation("🔍 Verifying candidate access with token: {Token}",
                request.Token.Substring(0, Math.Min(8, request.Token.Length)) + "...");

            var result = await _authService.VerifyCandidateAccessAsync(request.Token);

            if (result.IsSuccess)
            {
                _logger.LogInformation("✅ Candidate access verified for {Email}", result.Email);

                return Ok(new AuthResponseExtended
                {
                    Token = result.AuthToken,
                    Expiry = result.Expiry,
                    UserRole = UserRole.Candidate,
                    Email = result.Email,
                    Nom = result.Nom,
                    Prenom = result.Prenom,
                    RequiresEmailInvitation = false,
                    AvailableTests = result.AvailableTests
                });
            }

            _logger.LogWarning("❌ Candidate access verification failed: {Error}", result.ErrorMessage);
            return BadRequest(new { error = result.ErrorMessage });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la vérification d'accès candidat");
            return StatusCode(500, new { error = "Une erreur s'est produite" });
        }
    }

    // Endpoint de test simple pour vérifier que le contrôleur fonctionne
    [HttpGet("test")]
    public IActionResult Test()
    {
        return Ok(new { message = "AuthController is working!", timestamp = DateTime.Now });
    }
}