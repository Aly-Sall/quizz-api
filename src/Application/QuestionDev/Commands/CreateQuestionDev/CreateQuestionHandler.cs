// src/Application/QuestionDev/Commands/CreateQuestionDev/CreateQuestionHandler.cs - VERSION FINALE CORRIGÉE
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

    public CreateQuestionCommandHandler(
        IApplicationDbContext context,
        IValidator<CreateQuestionCommand> validator)
    {
        _context = context;
        _validator = validator;
    }

    public async Task<Result> Handle(CreateQuestionCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // 1. Valider la commande
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                return Result.Failure(errors);
            }

            // 2. Vérifier que le test existe
            var existingTest = await _context.Tests
                .FirstOrDefaultAsync(t => t.Id == request.QuizTestId, cancellationToken);

            if (existingTest == null)
            {
                return Result.Failure($"Test with ID {request.QuizTestId} does not exist");
            }

            // 3. Valider les choix
            if (request.Choices == null || !request.Choices.Any())
            {
                return Result.Failure("At least one choice is required");
            }

            if (request.Choices.Count < 2)
            {
                return Result.Failure("At least 2 choices are required");
            }

            // 4. Valider les réponses correctes
            if (string.IsNullOrWhiteSpace(request.ListOfCorrectAnswerIds) ||
                request.ListOfCorrectAnswerIds == "[]")
            {
                return Result.Failure("At least one correct answer must be specified");
            }

            // 5. ✅ CRÉER LA QUESTION avec l'association correcte au test
            var newQuestion = new Question
            {
                Content = request.Content.Trim(),
                Type = request.Type,
                AnswerDetails = request.AnswerDetails?.Trim() ?? string.Empty,
                QuizTestId = request.QuizTestId, // ✅ AJOUTÉ : Association avec le test
                ListOfCorrectAnswerIds = request.ListOfCorrectAnswerIds,
                Choices = request.Choices.ToArray()
            };

            // 6. Sauvegarder en base
            await _context.Questions.AddAsync(newQuestion, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success(newQuestion.Id);
        }
        catch (Exception ex)
        {
            return Result.Failure("An error occurred while creating the question. Please try again.");
        }
    }
}