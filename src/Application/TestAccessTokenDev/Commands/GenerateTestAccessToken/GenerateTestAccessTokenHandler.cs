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

namespace _Net6CleanArchitectureQuizzApp.Application.TestAccessTokenDev.Commands.GenerateTestAccessToken;
public class GenerateTestAccessTokenHandler : IRequestHandler<GenerateTokenAccessCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly IValidator<GenerateTokenAccessCommand> _validator;


    public GenerateTestAccessTokenHandler(IApplicationDbContext context, IValidator<GenerateTokenAccessCommand> validator)
    {
        _context = context;
        _validator = validator;
    }
    public async Task<Result> Handle(GenerateTokenAccessCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result.Failure(string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));
        }
        try
        {

            TestAccessToken TestToken = new TestAccessToken
            {
                CandidateEmail = request.CandidateEmail,
                ExpirationTime = DateTime.UtcNow.AddHours(request.ExpirationTime),
                IsUsed = request.IsUsed,
                TestId = request.TestId,
                TentativeId = request.TentativeId
            };

            await _context.TestAccessTokens.AddAsync(TestToken, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success(TestToken.Id);

        }
        catch (Exception)
        {
            return Result.Failure("An error occurred while Generating the Token");

        }

    }
}
