// src/WebUI/Controllers/AuthTestController.cs
using Microsoft.AspNetCore.Mvc;
using _Net6CleanArchitectureQuizzApp.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace _Net6CleanArchitectureQuizzApp.WebUI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthTestController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthTestController> _logger;

    public AuthTestController(
        IAuthService authService,
        ILogger<AuthTestController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    [HttpGet("check")]
    public IActionResult CheckAuthService()
    {
        try
        {
            var result = new
            {
                AuthServiceAvailable = _authService != null,
                AuthServiceType = _authService?.GetType().Name,
                Message = _authService != null
                    ? "✅ AuthService is properly configured"
                    : "❌ AuthService is not available"
            };

            _logger.LogInformation("Auth service check: {Available}", _authService != null);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking auth service");
            return StatusCode(500, new { Error = ex.Message });
        }
    }

    [HttpPost("test-register")]
    public async Task<IActionResult> TestRegister([FromBody] TestAuthRequest request)
    {
        try
        {
            var email = request.Email ?? $"test_{DateTime.Now.Ticks}@example.com";
            var password = request.Password ?? "test123";

            _logger.LogInformation("Testing registration for: {Email}", email);

            var result = await _authService.RegisterAsync(email, password, "Test", "User");

            return Ok(new
            {
                Success = result.IsSuccess,
                Message = result.IsSuccess ? "✅ Registration successful" : "❌ Registration failed",
                UserId = result.UserId,
                Email = result.Email,
                ErrorMessage = result.ErrorMessage,
                Errors = result.Errors
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during test registration");
            return StatusCode(500, new { Error = ex.Message });
        }
    }

    [HttpPost("test-login")]
    public async Task<IActionResult> TestLogin([FromBody] TestAuthRequest request)
    {
        try
        {
            if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
            {
                return BadRequest(new { Error = "Email and Password are required" });
            }

            _logger.LogInformation("Testing login for: {Email}", request.Email);

            var result = await _authService.LoginAsync(request.Email, request.Password);

            return Ok(new
            {
                Success = result.IsSuccess,
                Message = result.IsSuccess ? "✅ Login successful" : "❌ Login failed",
                Token = result.Token,
                Email = result.Email,
                UserName = result.UserName,
                UserId = result.UserId,
                ErrorMessage = result.ErrorMessage
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during test login");
            return StatusCode(500, new { Error = ex.Message });
        }
    }
}

public class TestAuthRequest
{
    public string? Email { get; set; }
    public string? Password { get; set; }
}