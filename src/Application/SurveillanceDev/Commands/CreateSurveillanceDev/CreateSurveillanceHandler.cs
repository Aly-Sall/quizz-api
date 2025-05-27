using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _Net6CleanArchitectureQuizzApp.Application.Common.Interfaces;
using _Net6CleanArchitectureQuizzApp.Application.Common.Models;
using _Net6CleanArchitectureQuizzApp.Application.QuestionDev.Commands.CreateQuestionDev;
using _Net6CleanArchitectureQuizzApp.Domain.Entities;
using FluentValidation;
using MediatR;

namespace _Net6CleanArchitectureQuizzApp.Application.SurveillanceDev.Commands.CreateSurveillanceDev;
public class CreateSurveillanceHandler : IRequestHandler<CreateSurveillanceCommand,Result>
{
    private readonly IApplicationDbContext _context;
    private readonly IValidator<CreateSurveillanceCommand> _validator;


    public CreateSurveillanceHandler(IApplicationDbContext context, IValidator<CreateSurveillanceCommand> validator)
    {
        _context = context;
        _validator = validator;
    }
    public async Task<Result> Handle(CreateSurveillanceCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result.Failure(string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));
        }
        try
        {
            Surveillance surveillance = new Surveillance
            {
                TentativeId = request.TentativeId,
                CaptureEcran = request.CaptureEcran,
            };

            await _context.Surveillances.AddAsync(surveillance, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success(surveillance.Id);

        }
        catch (Exception)
        {
            return Result.Failure("An error occurred while creating the Question");

        }
    }
}
