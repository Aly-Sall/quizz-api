using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _Net6CleanArchitectureQuizzApp.Application.Common.Interfaces;
using _Net6CleanArchitectureQuizzApp.Application.Common.Models;
using _Net6CleanArchitectureQuizzApp.Domain.Entities;
using _Net6CleanArchitectureQuizzApp.Domain.Enums;
using FluentValidation;
using MediatR;



namespace _Net6CleanArchitectureQuizzApp.Application.TestDev.Commands.CreateQuizTest.CreateTest;
public class CreateTestCommand : IRequest<Result>
{
    public string? Title { get; set; }
    public Category Category { get; set; }
    public Mode Mode { get; set; }
    public bool TryAgain { get; set; }
    public bool ShowTimer { get; set; }
    public Level Level { get; set; }
}

public class CreateTestCommandHandler : IRequestHandler<CreateTestCommand,Result>
{
    private readonly IApplicationDbContext _context;
    private readonly IValidator<CreateTestCommand> _validator;


    public CreateTestCommandHandler(IApplicationDbContext context, IValidator<CreateTestCommand> validator)
    {
        _context = context;
        _validator = validator;
    }
    public async Task<Result> Handle(CreateTestCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result.Failure(string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));
        }
        try
        {
            QuizTest newTest = new QuizTest
            {
                Title = request.Title,
                Category = request.Category,
                Mode = request.Mode,
                TryAgain = request.TryAgain,
                ShowTimer = request.ShowTimer,
                Level = request.Level,
            };
            await _context.Tests.AddAsync(newTest);
            await _context.SaveChangesAsync(cancellationToken);
            return Result.Success(newTest.Id);

        }
        catch (Exception ex) 
        {
            return Result.Failure("An error occurred while creating the Test");

        }


    }
}
