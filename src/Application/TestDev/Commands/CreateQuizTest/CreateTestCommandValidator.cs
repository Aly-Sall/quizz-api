using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _Net6CleanArchitectureQuizzApp.Application.TestDev.Commands.CreateQuizTest.CreateTest;
using FluentValidation;

namespace _Net6CleanArchitectureQuizzApp.Application.TestDev.Commands.CreateQuizTest;
public class CreateTestCommandValidator : AbstractValidator<CreateTestCommand>
{
    public CreateTestCommandValidator()
    {
        RuleFor(v => v.Title)
            .MaximumLength(100)
            .NotEmpty();
        RuleFor(v => v.ShowTimer)
            .NotNull();
        RuleFor(v => v.TryAgain)
          .NotNull();
    }

}
