using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _Net6CleanArchitectureQuizzApp.Application.Common.Interfaces;
using _Net6CleanArchitectureQuizzApp.Application.Common.Models;
using _Net6CleanArchitectureQuizzApp.Application.SurveillanceDev.Commands.CreateSurveillanceDev;
using _Net6CleanArchitectureQuizzApp.Domain.Entities;
using _Net6CleanArchitectureQuizzApp.Domain.Enums;
using FluentValidation;
using MediatR;

namespace _Net6CleanArchitectureQuizzApp.Application.TentativeDev.Commands.CreateTentative;

public class CreateTentativeHandler : IRequestHandler<CreateTentativeCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly IValidator<CreateTentativeCommand> _validator;


    public CreateTentativeHandler(IApplicationDbContext context, IValidator<CreateTentativeCommand> validator)
    {
        _context = context;
        _validator = validator;
    }
    public async Task<Result> Handle(CreateTentativeCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result.Failure(string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));
        }
        try
        {
            Tentative newTentative = new Tentative
            {
                PassingDate = request.PassingDate,
                TestId = request.TestId,
            };

            await _context.Tentatives.AddAsync(newTentative, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success(newTentative.Id);

        }
        catch (Exception)
        {
            return Result.Failure("An error occurred while creating the Question");

        }
    }
}

