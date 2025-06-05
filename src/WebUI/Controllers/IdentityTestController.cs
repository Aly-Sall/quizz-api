// src/WebUI/Controllers/IdentityTestController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using _Net6CleanArchitectureQuizzApp.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace _Net6CleanArchitectureQuizzApp.WebUI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class IdentityTestController : ControllerBase
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly ILogger<IdentityTestController> _logger;

    public IdentityTestController(
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        ILogger<IdentityTestController> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _logger = logger;
    }

    [HttpGet("check-services")]
    public IActionResult CheckServices()
    {
        try
        {
            var result = new
            {
                UserManagerAvailable = _userManager != null,
                SignInManagerAvailable = _signInManager != null,
                UserManagerType = _userManager?.GetType().Name,
                SignInManagerType = _signInManager?.GetType().Name,
                PasswordOptions = new
                {
                    RequireDigit = _userManager?.Options.Password.RequireDigit,
                    RequiredLength = _userManager?.Options.Password.RequiredLength,
                    RequireNonAlphanumeric = _userManager?.Options.Password.RequireNonAlphanumeric,
                    RequireUppercase = _userManager?.Options.Password.RequireUppercase,
                    RequireLowercase = _userManager?.Options.Password.RequireLowercase
                }
            };

            _logger.LogInformation("✅ Identity services check completed successfully");
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error checking Identity services");
            return StatusCode(500, new { Error = ex.Message });
        }
    }

    [HttpPost("test-user-creation")]
    public async Task<IActionResult> TestUserCreation([FromBody] TestUserRequest request)
    {
        try
        {
            var testEmail = $"test_{DateTime.Now.Ticks}@example.com";
            var password = request.Password ?? "Test123!";

            _logger.LogInformation("🔍 Creating test user: {Email}", testEmail);

            var user = new User
            {
                Email = testEmail,
                UserName = testEmail,
                EmailConfirmed = true,
                LockoutEnabled = false
            };

            var createResult = await _userManager.CreateAsync(user, password);

            if (createResult.Succeeded)
            {
                _logger.LogInformation("✅ User created successfully");

                // Test immediate password verification
                var passwordCheck = await _userManager.CheckPasswordAsync(user, password);
                var signInCheck = await _signInManager.CheckPasswordSignInAsync(user, password, false);

                // Clean up - delete test user
                await _userManager.DeleteAsync(user);
                _logger.LogInformation("🗑️ Test user deleted");

                return Ok(new
                {
                    Success = true,
                    UserCreated = true,
                    PasswordCheckPassed = passwordCheck,
                    SignInCheckPassed = signInCheck.Succeeded,
                    SignInResult = new
                    {
                        signInCheck.Succeeded,
                        signInCheck.IsLockedOut,
                        signInCheck.IsNotAllowed,
                        signInCheck.RequiresTwoFactor
                    }
                });
            }
            else
            {
                var errors = createResult.Errors.Select(e => $"{e.Code}: {e.Description}");
                _logger.LogError("❌ User creation failed: {Errors}", string.Join(", ", errors));

                return Ok(new
                {
                    Success = false,
                    UserCreated = false,
                    Errors = errors
                });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error during user creation test");
            return StatusCode(500, new { Error = ex.Message, StackTrace = ex.StackTrace });
        }
    }

    [HttpPost("test-existing-user")]
    public async Task<IActionResult> TestExistingUser([FromBody] ExistingUserTestRequest request)
    {
        try
        {
            _logger.LogInformation("🔍 Testing existing user: {Email}", request.Email);

            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                user = await _userManager.FindByNameAsync(request.Email);
            }

            if (user == null)
            {
                return Ok(new { Found = false, Message = "User not found" });
            }

            var passwordCheck = await _userManager.CheckPasswordAsync(user, request.Password);
            var signInCheck = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);

            return Ok(new
            {
                Found = true,
                UserId = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                EmailConfirmed = user.EmailConfirmed,
                LockoutEnabled = user.LockoutEnabled,
                AccessFailedCount = user.AccessFailedCount,
                HasPasswordHash = !string.IsNullOrEmpty(user.PasswordHash),
                PasswordHashLength = user.PasswordHash?.Length ?? 0,
                PasswordCheckPassed = passwordCheck,
                SignInCheckPassed = signInCheck.Succeeded,
                SignInResult = new
                {
                    signInCheck.Succeeded,
                    signInCheck.IsLockedOut,
                    signInCheck.IsNotAllowed,
                    signInCheck.RequiresTwoFactor
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error testing existing user");
            return StatusCode(500, new { Error = ex.Message });
        }
    }
}

public class TestUserRequest
{
    public string? Password { get; set; }
}

public class ExistingUserTestRequest
{
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
}