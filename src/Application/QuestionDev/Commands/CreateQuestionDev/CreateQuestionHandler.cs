using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _Net6CleanArchitectureQuizzApp.Application.Common.Interfaces;
using _Net6CleanArchitectureQuizzApp.Application.Common.Models;
using _Net6CleanArchitectureQuizzApp.Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace _Net6CleanArchitectureQuizzApp.Application.QuestionDev.Commands.CreateQuestionDev;
public class CreateQuestionCommandHandler : IRequestHandler<CreateQuestionCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly IValidator<CreateQuestionCommand> _validator;


    public CreateQuestionCommandHandler(IApplicationDbContext context, IValidator<CreateQuestionCommand> validator)
    {
        _context = context;
        _validator = validator;
    }
    public async Task<Result> Handle(CreateQuestionCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result.Failure(string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));
        }
        try
        {
            var existedTest = await _context.Tests.FirstOrDefaultAsync(t => t.Id == request.QuizTestId);
            if (existedTest==null)
            {
                return Result.Failure("Test doesn't exist in database");

            }
            Question newQuestion = new Question
            {
                Content = request.Content,
                Type = request.Type,
                AnswerDetails = request.AnswerDetails,
                ListOfCorrectAnswerIds = request.ListOfCorrectAnswerIds
            };

            await _context.Questions.AddAsync(newQuestion, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success(newQuestion.Id);

        }
        catch (Exception)
        {
            return Result.Failure("An error occurred while creating the Question");

        }


    }
}
