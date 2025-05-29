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
using Microsoft.Extensions.Logging;

namespace _Net6CleanArchitectureQuizzApp.Application.QuestionDev.Commands.CreateQuestionDev;

public class CreateQuestionCommandHandler : IRequestHandler<CreateQuestionCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly IValidator<CreateQuestionCommand> _validator;
    private readonly ILogger<CreateQuestionCommandHandler> _logger;

    public CreateQuestionCommandHandler(
        IApplicationDbContext context,
        IValidator<CreateQuestionCommand> validator,
        ILogger<CreateQuestionCommandHandler> logger)
    {
        _context = context;
        _validator = validator;
        _logger = logger;
    }

    public async Task<Result> Handle(CreateQuestionCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Creating question with content: {Content} for test: {TestId}",
                request.Content, request.QuizTestId);

            // Validation
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                _logger.LogWarning("Validation failed: {Errors}", errors);
                return Result.Failure(errors);
            }

            // Vérifier que le test existe
            var existedTest = await _context.Tests
                .Include(t => t.Questions) // Inclure les questions pour la relation
                .FirstOrDefaultAsync(t => t.Id == request.QuizTestId, cancellationToken);

            if (existedTest == null)
            {
                _logger.LogWarning("Test with ID {TestId} not found", request.QuizTestId);
                return Result.Failure("Test doesn't exist in database");
            }

            // Valider les choix
            if (request.Choices == null || !request.Choices.Any())
            {
                _logger.LogWarning("No choices provided for question");
                return Result.Failure("Question must have at least one choice");
            }

            // Créer la nouvelle question
            var newQuestion = new Question
            {
                Content = request.Content?.Trim() ?? string.Empty,
                Type = request.Type,
                AnswerDetails = request.AnswerDetails?.Trim(),
                ListOfCorrectAnswerIds = request.ListOfCorrectAnswerIds ?? "[]",
                // Initialiser les collections
                QuizTests = new List<QuizTest>(),
                Reponses = new List<Reponse>()
            };

            // IMPORTANT: Assigner les choix APRÈS la création de l'objet
            // pour éviter les problèmes de sérialisation avec _Choices
            try
            {
                newQuestion.Choices = request.Choices.ToArray();
                _logger.LogInformation("Assigned {ChoiceCount} choices to question", request.Choices.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error serializing choices");
                return Result.Failure("Error processing question choices");
            }

            // Établir la relation many-to-many
            newQuestion.QuizTests.Add(existedTest);
            // Ou alternativement : existedTest.Questions.Add(newQuestion);

            // Sauvegarder la question
            await _context.Questions.AddAsync(newQuestion, cancellationToken);

            _logger.LogInformation("Saving question to database...");
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Question created successfully with ID: {QuestionId}", newQuestion.Id);
            return Result.Success(newQuestion.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating question for test {TestId}: {Message}",
                request.QuizTestId, ex.Message);

            // Retourner des détails plus spécifiques selon le type d'erreur
            if (ex.InnerException != null)
            {
                _logger.LogError("Inner exception: {InnerMessage}", ex.InnerException.Message);
            }

            return Result.Failure($"An error occurred while creating the question: {ex.Message}");
        }
    }
}