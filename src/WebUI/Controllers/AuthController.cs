// src/WebUI/Controllers/AuthController.cs - CORRIGÉ POUR ENVOYER EMAIL AU CANDIDAT
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

            // Vérifier d'abord si l'utilisateur existe et si le mot de passe est correct
            var result = await _authService.LoginWithRoleAsync(request.Email, request.Password, request.UserRole);

            if (result.IsSuccess)
            {
                if (request.UserRole == UserRole.Administrator)
                {
                    // ✅ Admin - retourner directement le token JWT
                    _logger.LogInformation("✅ Admin login successful for {Email}", request.Email);
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
                    // 📧 Candidat - NE PAS donner de token, mais envoyer un email d'invitation
                    _logger.LogInformation("🔍 Candidate login - sending invitation email to {Email}", request.Email);

                    var emailSent = await _candidateInvitationService.SendCandidateInvitationEmailAsync(
                        result.Email!,
                        $"{result.Prenom} {result.Nom}");

                    if (emailSent)
                    {
                        _logger.LogInformation("✅ Invitation email sent successfully to {Email}", request.Email);
                        return Ok(new AuthResponseExtended
                        {
                            Message = "Un email d'invitation vous a été envoyé. Veuillez vérifier votre boîte mail pour accéder à vos tests.",
                            Email = result.Email,
                            RequiresEmailInvitation = true,
                            UserRole = result.UserRole,
                            // ❌ PAS de Token ici - le candidat doit passer par l'email
                            Token = null,
                            Expiry = null
                        });
                    }
                    else
                    {
                        _logger.LogError("❌ Failed to send invitation email to {Email}", request.Email);
                        return StatusCode(500, new { error = "Erreur lors de l'envoi de l'email d'invitation" });
                    }
                }
            }

            _logger.LogWarning("❌ Login failed for {Email}: {Error}", request.Email, result.ErrorMessage);
            return BadRequest(new { error = result.ErrorMessage });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Erreur lors de la connexion pour {Email}", request.Email);
            return StatusCode(500, new { error = "Une erreur s'est produite lors de la connexion" });
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

                // ✅ Maintenant on peut donner le token JWT au candidat
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
            _logger.LogError(ex, "❌ Erreur lors de la vérification d'accès candidat");
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