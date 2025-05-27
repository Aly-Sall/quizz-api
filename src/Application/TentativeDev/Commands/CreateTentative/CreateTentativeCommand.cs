using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _Net6CleanArchitectureQuizzApp.Application.Common.Models;
using FluentValidation;
using MediatR;

namespace _Net6CleanArchitectureQuizzApp.Application.TentativeDev.Commands.CreateTentative;
public class CreateTentativeCommand : IRequest<Result>
{
    public DateTime PassingDate { get; set; }
    public int TestId { get; set; }
}

public class CreateTentativeCommandValidator : AbstractValidator<CreateTentativeCommand>
{
    public CreateTentativeCommandValidator()
    {
         RuleFor(v => v.TestId)
            .NotEmpty()
            .NotEqual(-1);
        RuleFor(v => v.PassingDate)
          .NotNull();
    }

}