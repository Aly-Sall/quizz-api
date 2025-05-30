using _Net6CleanArchitectureQuizzApp.Application.Account.Commands.Login;
using _Net6CleanArchitectureQuizzApp.Application.Account.Commands.Register;
using _Net6CleanArchitectureQuizzApp.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace _Net6CleanArchitectureQuizzApp.WebUI.Controllers
{
    public class AccountController : ApiControllerBase
    {
        private readonly IMediator _mediator;
        private readonly UserManager<User> _userManager;

        public AccountController(IMediator mediator, UserManager<User> userManager)
        {
            _mediator = mediator;
            _userManager = userManager;
        }

        /// <summary>
        /// Registers a new user.
        /// </summary>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserModel model)
        {
            var result = await _mediator.Send(model);

            if (result.IsSuccess)
                return Ok(result);

            return BadRequest(result);
        }

        /// <summary>
        /// Logs in a user and returns a JWT token.
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            try
            {
                var authResponse = await _mediator.Send(model);
                return Ok(authResponse);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Debug endpoint pour tester l'authentification
        /// </summary>
        [HttpPost("debug-login")]
        public async Task<IActionResult> DebugLogin([FromBody] LoginModel model)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    user = await _userManager.FindByNameAsync(model.Email);
                }

                if (user == null)
                    return Ok(new { found = false, message = "User not found by email or username" });

                var passwordCheck = await _userManager.CheckPasswordAsync(user, model.Password);
                var isLockedOut = await _userManager.IsLockedOutAsync(user);

                return Ok(new
                {
                    found = true,
                    passwordValid = passwordCheck,
                    userId = user.Id,
                    email = user.Email,
                    userName = user.UserName,
                    emailConfirmed = user.EmailConfirmed,
                    lockoutEnabled = user.LockoutEnabled,
                    isLockedOut = isLockedOut,
                    accessFailedCount = user.AccessFailedCount
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message, stackTrace = ex.StackTrace });
            }
        }
    }
}