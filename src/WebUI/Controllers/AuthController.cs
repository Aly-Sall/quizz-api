// src/WebUI/Controllers/AuthController.cs - Nouveau
using _Net6CleanArchitectureQuizzApp.Application.Common.Interfaces;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IEmailService _emailService;
    private readonly ITestInvitationService _testInvitationService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        IAuthService authService,
        IEmailService emailService,
        ITestInvitationService testInvitationService,
        ILogger<AuthController> logger)
    {
        _authService = authService;
        _emailService = emailService;
        _testInvitationService = testInvitationService;
        _logger = logger;
    }

    [HttpPost("login-with-role")]
    public async Task<IActionResult> LoginWithRole([FromBody] LoginWithRoleRequest request)
    {
        try
        {
            var result = await _authService.LoginWithRoleAsync(request.Email, request.Password, request.UserRole);

            if (result.IsSuccess)
            {
                if (request.UserRole == UserRole.Administrator)
                {
                    // Admin - retourner directement le token
                    return Ok(new AuthResponse
                    {
                        Token = result.Token,
                        Expiry = result.Expiry.Value,
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
                    await _testInvitationService.SendCandidateInvitationEmailAsync(result.Email, result.Nom);

                    return Ok(new AuthResponse
                    {
                        Message = "Un email d'invitation vous a été envoyé. Veuillez vérifier votre boîte mail.",
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
            var result = await _authService.VerifyCandidateAccessAsync(request.Token);

            if (result.IsSuccess)
            {
                return Ok(new AuthResponse
                {
                    Token = result.AuthToken,
                    Expiry = result.Expiry.Value,
                    UserRole = UserRole.Candidate,
                    Email = result.Email,
                    Nom = result.Nom,
                    Prenom = result.Prenom,
                    RequiresEmailInvitation = false,
                    AvailableTests = result.AvailableTests
                });
            }

            return BadRequest(new { error = result.ErrorMessage });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la vérification d'accès candidat");
            return StatusCode(500, new { error = "Une erreur s'est produite" });
        }
    }
}

// Modèles de requête
public class LoginWithRoleRequest
{
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public UserRole UserRole { get; set; }
}

public class VerifyCandidateAccessRequest
{
    public string Token { get; set; } = null!;
}

public class AuthResponse
{
    public string? Token { get; set; }
    public DateTime? Expiry { get; set; }
    public UserRole? UserRole { get; set; }
    public string? Email { get; set; }
    public string? Nom { get; set; }
    public string? Prenom { get; set; }
    public string? Message { get; set; }
    public bool RequiresEmailInvitation { get; set; }
    public List<TestDto>? AvailableTests { get; set; }
}