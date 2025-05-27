using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _Net6CleanArchitectureQuizzApp.Application.Common.Models;
using _Net6CleanArchitectureQuizzApp.Application.TestDev.Commands.CreateQuizTest.CreateTest;
using _Net6CleanArchitectureQuizzApp.Domain.Enums;
using FluentValidation;
using MediatR;

namespace _Net6CleanArchitectureQuizzApp.Application.QuestionDev.Commands.CreateQuestionDev;


public class CreateQuestionCommand : IRequest<Result>
{
    public string Content { get; set; } = null!;
    public QuestionType Type { get; set; }
    public string? AnswerDetails { get; set; }
    public int QuizTestId { get; set; }
    public string ListOfCorrectAnswerIds { get; set; }
}

public class CreateQuestionCommandValidator : AbstractValidator<CreateQuestionCommand>
{
    public CreateQuestionCommandValidator()
    {
        RuleFor(v => v.Content)
            .NotEmpty();
        RuleFor(v => v.QuizTestId)
            .NotNull();
        RuleFor(v => v.ListOfCorrectAnswerIds)
          .NotEmpty();
    }

}


