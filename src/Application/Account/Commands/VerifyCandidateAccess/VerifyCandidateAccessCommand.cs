// src/Application/Account/Commands/VerifyCandidateAccess/VerifyCandidateAccessCommand.cs
using _Net6CleanArchitectureQuizzApp.Application.Account.Models;
using MediatR;

namespace _Net6CleanArchitectureQuizzApp.Application.Account.Commands.VerifyCandidateAccess;

public class VerifyCandidateAccessCommand : IRequest<AuthResponseExtended>
{
    public string Token { get; set; } = null!;
}