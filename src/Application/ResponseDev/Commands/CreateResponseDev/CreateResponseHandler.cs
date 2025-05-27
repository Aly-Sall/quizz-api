using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _Net6CleanArchitectureQuizzApp.Application.Common.Interfaces;
using _Net6CleanArchitectureQuizzApp.Application.Common.Models;
using _Net6CleanArchitectureQuizzApp.Application.TestAccessTokenDev.Commands.GenerateTestAccessToken;
using _Net6CleanArchitectureQuizzApp.Domain.Entities;
using FluentValidation;
using MediatR;

namespace _Net6CleanArchitectureQuizzApp.Application.ResponseDev.Commands.CreateResponseDev;
public class CreateResponseHandler : IRequestHandler<CreateResponseCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly IValidator<CreateResponseCommand> _validator;


    public CreateResponseHandler(IApplicationDbContext context, IValidator<CreateResponseCommand> validator)
    {
        _context = context;
        _validator = validator;
    }
    public async Task<Result> Handle(CreateResponseCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result.Failure(string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));
        }
        try
        {
            Reponse CandidateAnswer = new Reponse
            {
                ChoiceId = request.ChoiceId,
                QuestionId = request.QuestionId,
                QuizTestId = request.QuizTestId,

            };

            await _context.Responses.AddAsync(CandidateAnswer, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success(CandidateAnswer.Id);
        }
        catch (Exception)
        {
            return Result.Failure("An error occurred while Saving the candidate answer");

        }
    }
}