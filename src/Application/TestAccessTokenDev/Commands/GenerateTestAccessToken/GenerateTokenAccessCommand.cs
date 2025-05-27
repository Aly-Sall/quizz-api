using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _Net6CleanArchitectureQuizzApp.Application.Common.Models;
using FluentValidation;
using MediatR;

namespace _Net6CleanArchitectureQuizzApp.Application.TestAccessTokenDev.Commands.GenerateTestAccessToken;
public class GenerateTokenAccessCommand : IRequest<Result>
{
    public string CandidateEmail { get; set; } = null!;
    public int ExpirationTime { get; set; }
    public bool IsUsed { get; set; } = false;
    public int TestId { get; set; }
    public int? TentativeId { get; set; }
}
public class GenerateTokenAccessCommandValidator : AbstractValidator<GenerateTokenAccessCommand>
{
    public GenerateTokenAccessCommandValidator()
    {
        RuleFor(v => v.ExpirationTime)
         .NotEmpty()
         .InclusiveBetween(1, 72);

        RuleFor(v => v.TestId)
         .NotNull();
       
        RuleFor(v => v.IsUsed)
         .NotEmpty();

        RuleFor(v => v.CandidateEmail)
         .NotEmpty();
    }

}
