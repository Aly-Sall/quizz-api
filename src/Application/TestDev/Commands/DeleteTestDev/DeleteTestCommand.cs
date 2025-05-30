// src/Application/TestDev/Commands/DeleteTestDev/DeleteTestCommand.cs - VERSION COMPLÈTE
using _Net6CleanArchitectureQuizzApp.Application.Common.Exceptions;
using _Net6CleanArchitectureQuizzApp.Application.Common.Interfaces;
using _Net6CleanArchitectureQuizzApp.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace _Net6CleanArchitectureQuizzApp.Application.TestDev.Commands.DeleteTestDev;

public record DeleteTestCommand(int Id) : IRequest;

public class DeleteTestCommandHandler : IRequestHandler<DeleteTestCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<DeleteTestCommandHandler> _logger;

    public DeleteTestCommandHandler(IApplicationDbContext context, ILogger<DeleteTestCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Unit> Handle(DeleteTestCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Deleting test with ID: {Id}", request.Id);

            // Récupérer le test avec ses questions
            var entity = await _context.Tests
                .Include(t => t.Questions) // Inclure les questions pour vérification
                .FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken);

            if (entity == null)
            {
                _logger.LogWarning("Test with ID {Id} not found", request.Id);
                throw new NotFoundException(nameof(QuizTest), request.Id);
            }

            // Vérifier si le test est actif - optionnel selon vos règles métier
            if (entity.IsActive)
            {
                _logger.LogWarning("Attempting to delete active test with ID {Id}", request.Id);
                throw new ForbiddenAccessException();
            }

            // Supprimer d'abord les questions associées si nécessaire
            if (entity.Questions != null && entity.Questions.Any())
            {
                _logger.LogInformation("Deleting {Count} associated questions for test {Id}",
                    entity.Questions.Count, request.Id);

                // Option 1: Supprimer en cascade (recommandé si configuré dans DbContext)
                // Les questions seront supprimées automatiquement grâce à DeleteBehavior.Cascade

                // Option 2: Supprimer manuellement si pas de cascade
                // _context.Questions.RemoveRange(entity.Questions);
            }

            // Supprimer le test
            _context.Tests.Remove(entity);

            var result = await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Successfully deleted test {Id} and {Count} associated questions",
                request.Id, entity.Questions?.Count ?? 0);

            return Unit.Value;
        }
        catch (NotFoundException)
        {
            // Re-throw les exceptions connues
            throw;
        }
        catch (ForbiddenAccessException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting test with ID: {Id}", request.Id);
            throw new InvalidOperationException($"An error occurred while deleting test {request.Id}", ex);
        }
    }
}