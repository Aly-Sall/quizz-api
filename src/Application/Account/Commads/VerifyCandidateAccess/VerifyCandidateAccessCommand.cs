using _Net6CleanArchitectureQuizzApp.Application.Account.Models;
using MediatR;

public class VerifyCandidateAccessCommand : IRequest<AuthResponseExtended>
{
    public string Token { get; set; } = null!;
}