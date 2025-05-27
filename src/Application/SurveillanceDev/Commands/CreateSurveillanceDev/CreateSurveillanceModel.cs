using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _Net6CleanArchitectureQuizzApp.Application.Common.Models;
using _Net6CleanArchitectureQuizzApp.Application.QuestionDev.Commands.CreateQuestionDev;
using _Net6CleanArchitectureQuizzApp.Domain.Entities;
using FluentValidation;
using MediatR;

namespace _Net6CleanArchitectureQuizzApp.Application.SurveillanceDev.Commands.CreateSurveillanceDev;
public class CreateSurveillanceCommand : IRequest<Result>
{   
    public string CaptureEcran { get; set; }
    public int TentativeId { get; set; }
}
public class CreateSurveillanceCommandValidator : AbstractValidator<CreateSurveillanceCommand>
{
    public CreateSurveillanceCommandValidator()
    {
        
        RuleFor(v => v.TentativeId)
            .NotEmpty()
            .NotEqual(-1);
        RuleFor(v => v.CaptureEcran)
          .NotNull();
        
    }

}

