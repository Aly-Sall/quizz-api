using _Net6CleanArchitectureQuizzApp.Application.Account.Commands.Login;
using _Net6CleanArchitectureQuizzApp.Application.Account.Commands.Register;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace _Net6CleanArchitectureQuizzApp.WebUI.Controllers;

public class AccountController : ApiControllerBase
{
    private readonly IMediator _mediator;

    public AccountController(IMediator mediator)
    {
        _mediator = mediator;
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
        catch (UnauthorizedAccessException)
        {
            return Unauthorized("Invalid email or password.");
        }
    }
}
