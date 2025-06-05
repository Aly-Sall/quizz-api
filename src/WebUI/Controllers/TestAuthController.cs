using _Net6CleanArchitectureQuizzApp.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace _Net6CleanArchitectureQuizzApp.WebUI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestAuthController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly ILogger<TestAuthController> _logger;

        public TestAuthController(UserManager<User> userManager, ILogger<TestAuthController> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        [HttpPost("check-user")]
        public async Task<IActionResult> CheckUser([FromBody] CheckUserRequest request)
        {
            try
            {
                _logger.LogInformation("🔍 Checking user: {Email}", request.Email);

                // Chercher l'utilisateur
                var user = await _userManager.FindByEmailAsync(request.Email);
                if (user == null)
                {
                    user = await _userManager.FindByNameAsync(request.Email);
                }

                if (user == null)
                {
                    return Ok(new { found = false, message = "Utilisateur non trouvé" });
                }

                // Test du mot de passe
                var passwordValid = await _userManager.CheckPasswordAsync(user, request.Password);

                var result = new
                {
                    found = true,
                    passwordValid = passwordValid,
                    user = new
                    {
                        id = user.Id,
                        email = user.Email,
                        userName = user.UserName,
                        nom = user.Nom,
                        prenom = user.Prenom,
                        emailConfirmed = user.EmailConfirmed,
                        lockoutEnabled = user.LockoutEnabled,
                        accessFailedCount = user.AccessFailedCount,
                        hasPasswordHash = !string.IsNullOrEmpty(user.PasswordHash),
                        passwordHashLength = user.PasswordHash?.Length ?? 0
                    }
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors du test utilisateur");
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost("create-simple-user")]
        public async Task<IActionResult> CreateSimpleUser([FromBody] CreateUserRequest request)
        {
            try
            {
                // Vérifier si l'utilisateur existe
                var existing = await _userManager.FindByEmailAsync(request.Email);
                if (existing != null)
                {
                    return BadRequest(new { error = "Utilisateur déjà existant" });
                }

                // Créer un utilisateur simple
                var user = new User
                {
                    Email = request.Email,
                    UserName = request.Email,
                    EmailConfirmed = true,
                    LockoutEnabled = false,
                    Nom = "Test",
                    Prenom = "User"
                };

                var result = await _userManager.CreateAsync(user, request.Password);

                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select(e => e.Description);
                    return BadRequest(new { errors = errors });
                }

                // Test immédiat du mot de passe
                var createdUser = await _userManager.FindByEmailAsync(request.Email);
                var passwordCheck = await _userManager.CheckPasswordAsync(createdUser, request.Password);

                return Ok(new
                {
                    success = true,
                    userId = createdUser.Id,
                    passwordCheckImmediate = passwordCheck,
                    message = "Utilisateur créé avec succès"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la création utilisateur");
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("list-users")]
        public async Task<IActionResult> ListUsers()
        {
            try
            {
                var users = await _userManager.Users
                    .Take(10)
                    .Select(u => new
                    {
                        id = u.Id,
                        email = u.Email,
                        userName = u.UserName,
                        nom = u.Nom,
                        prenom = u.Prenom,
                        hasPasswordHash = !string.IsNullOrEmpty(u.PasswordHash)
                    })
                    .ToListAsync();

                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }

    public class CheckUserRequest
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
    }

    public class CreateUserRequest
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}