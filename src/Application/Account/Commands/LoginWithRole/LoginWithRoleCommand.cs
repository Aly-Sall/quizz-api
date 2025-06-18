using _Net6CleanArchitectureQuizzApp.Domain.Enums;
using _Net6CleanArchitectureQuizzApp.Application.Account.Models;
using MediatR;

namespace _Net6CleanArchitectureQuizzApp.Application.Account.Commands.LoginWithRole;

public class LoginWithRoleCommand : IRequest<AuthResponseExtended>
{
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public UserRole UserRole { get; set; }
}
