using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _Net6CleanArchitectureQuizzApp.Application.Common.Models;
using FluentValidation;
using MediatR;

namespace _Net6CleanArchitectureQuizzApp.Application.ResponseDev.Commands.CreateResponseDev;
public class CreateResponseCommand : IRequest<Result>
{
    public int ChoiceId { get; set; }

    public int QuestionId { get; set; }
    public int QuizTestId { get; set; }

}
public class CreateResponseCommandValidator : AbstractValidator<CreateResponseCommand>
{
    public CreateResponseCommandValidator()
    {
        RuleFor(v => v.ChoiceId)
         .NotEqual(-1);

        RuleFor(v => v.QuestionId)
         .NotEqual(-1);
       
        RuleFor(v => v.QuizTestId)
         .NotEqual(-1);
    }
}
