using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _Net6CleanArchitectureQuizzApp.Application.Common.Models;
using _Net6CleanArchitectureQuizzApp.Application.TestAccessTokenDev.Commands.GenerateTestAccessToken;
using FluentValidation;
using MediatR;

namespace _Net6CleanArchitectureQuizzApp.Application.TestAccessTokenDev.Commands.UpdateTestAccessToken;
public class UpdateTestAccessTokenCommand : IRequest
{
    public int Id { get; set; }
    public bool IsUsed { get; set; }
    public int? TentativeId { get; set; }

}
public class UpdateTestAccessTokenCommandValidator : AbstractValidator<UpdateTestAccessTokenCommand>
{
    public UpdateTestAccessTokenCommandValidator()
    {

        RuleFor(v => v.IsUsed)
         .NotNull();

        RuleFor(v => v.Id)
         .NotEmpty();
    }

}
