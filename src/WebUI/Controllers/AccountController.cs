// src/WebUI/Controllers/AccountController.cs - FICHIER MANQUANT À CRÉER
using _Net6CleanArchitectureQuizzApp.Application.Account.Commands.Register;
using _Net6CleanArchitectureQuizzApp.Application.Account.Commands.Login;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using Microsoft.Extensions.Logging;

namespace _Net6CleanArchitectureQuizzApp.WebUI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountController : ApiControllerBase
{
    private readonly ILogger<AccountController> _logger;

    public AccountController(ILogger<AccountController> logger)
    {
        _logger = logger;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserModel model)
    {
        try
        {
            _logger.LogInformation("🔍 Registration request for: {Email}", model.Email);

            var result = await Mediator.Send(model);

            if (result.IsSuccess)
            {
                _logger.LogInformation("✅ Registration successful for: {Email}", model.Email);
                return Ok(new { isSuccess = true, message = "Utilisateur créé avec succès", id = result.Value });
            }
            else
            {
                _logger.LogWarning("❌ Registration failed for: {Email}, Errors: {Errors}",
                    model.Email, string.Join(", ", result.Errors ?? new string[] { result.Error ?? "Unknown error" }));
                return BadRequest(new { isSuccess = false, errors = result.Errors, error = result.Error });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Exception during registration for: {Email}", model.Email);
            return StatusCode(500, new { isSuccess = false, error = "Une erreur interne s'est produite" });
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginModel model)
    {
        try
        {
            _logger.LogInformation("🔍 Login request for: {Email}", model.Email);

            var result = await Mediator.Send(model);

            _logger.LogInformation("✅ Login successful for: {Email}", model.Email);
            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning("❌ Login failed for: {Email}, Reason: {Reason}", model.Email, ex.Message);
            return Unauthorized(new { isSuccess = false, error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Exception during login for: {Email}", model.Email);
            return StatusCode(500, new { isSuccess = false, error = "Une erreur interne s'est produite" });
        }
    }

    [HttpPost("debug-login")]
    public async Task<IActionResult> DebugLogin([FromBody] LoginModel model)
    {
        try
        {
            _logger.LogInformation("🔍 Debug login request for: {Email}", model.Email);

            var result = await Mediator.Send(model);

            _logger.LogInformation("✅ Debug login successful for: {Email}", model.Email);
            return Ok(new
            {
                isSuccess = true,
                token = result.Token,
                expiry = result.Expiry,
                email = result.Email,
                nom = result.Nom,
                prenom = result.Prenom,
                userName = result.UserName
            });
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning("❌ Debug login failed for: {Email}, Reason: {Reason}", model.Email, ex.Message);
            return Ok(new
            {
                isSuccess = false,
                error = ex.Message,
                email = model.Email
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Exception during debug login for: {Email}", model.Email);
            return Ok(new
            {
                isSuccess = false,
                error = "Une erreur interne s'est produite",
                exception = ex.Message
            });
        }
    }
}