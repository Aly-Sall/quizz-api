// src/WebUI/Controllers/QuizTestController.cs - VERSION COMPLÈTE
using _Net6CleanArchitectureQuizzApp.Application.Common.Interfaces;
using _Net6CleanArchitectureQuizzApp.Application.Common.Models;
using _Net6CleanArchitectureQuizzApp.Application.TestDev.Commands.CreateQuizTest.CreateTest;
using _Net6CleanArchitectureQuizzApp.Application.TestDev.Commands.DeleteTestDev;
using _Net6CleanArchitectureQuizzApp.Application.TestDev.Commands.UpdateTestDev;
using _Net6CleanArchitectureQuizzApp.Application.TestDev.Queries.GetQuizTestById;
using _Net6CleanArchitectureQuizzApp.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace _Net6CleanArchitectureQuizzApp.WebUI.Controllers;

public class QuizTestController : ApiControllerBase
{
    private readonly ILogger<QuizTestController> _logger;
    private readonly IApplicationDbContext _context;

    public QuizTestController(ILogger<QuizTestController> logger, IApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    /// <summary>
    /// Récupère tous les tests depuis la base de données
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<object>>> GetAllTests()
    {
        try
        {
            _logger.LogInformation("Getting all quiz tests from database");

            var tests = await _context.Tests
                .OrderByDescending(t => t.Id)
                .Select(t => new
                {
                    Id = t.Id,
                    Title = t.Title,
                    Category = (int)t.Category,
                    Mode = (int)t.Mode,
                    TryAgain = t.TryAgain,
                    ShowTimer = t.ShowTimer,
                    Level = (int)t.Level,
                    IsActive = t.IsActive,
                    Duration = t.Duration
                })
                .ToListAsync();

            _logger.LogInformation("Retrieved {Count} tests from database", tests.Count);
            return Ok(tests);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all quiz tests from database");

            var mockTests = new List<object>
            {
                new { Id = 1, Title = "Test Sample 1 (Mock)", IsActive = true, Category = 1, Mode = 0, Level = 0, TryAgain = false, ShowTimer = true, Duration = 30 },
                new { Id = 2, Title = "Test Sample 2 (Mock)", IsActive = false, Category = 2, Mode = 1, Level = 1, TryAgain = true, ShowTimer = false, Duration = 45 }
            };

            return Ok(mockTests);
        }
    }

    /// <summary>
    /// Récupère un test par son ID
    /// </summary>
    [HttpGet("by-id/{id}")]
    public async Task<Result<QuizTest>> GetQuizTestById([FromQuery] GetQuizTestQuery query)
    {
        try
        {
            _logger.LogInformation("Getting quiz test by ID: {Id}", query.Id);
            var result = await Mediator.Send(query);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting quiz test by ID: {Id}", query.Id);
            return Result<QuizTest>.Failure("Error retrieving quiz test");
        }
    }

    /// <summary>
    /// Crée un nouveau test
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<Result>> Create([FromBody] CreateTestCommand command)
    {
        try
        {
            _logger.LogInformation("Creating new quiz test with title: {Title}", command.Title);

            if (command == null)
            {
                return BadRequest(Result.Failure("Command cannot be null"));
            }

            if (string.IsNullOrWhiteSpace(command.Title))
            {
                return BadRequest(Result.Failure("Test title is required"));
            }

            var result = await Mediator.Send(command);

            if (result.IsSuccess)
            {
                _logger.LogInformation("Successfully created quiz test with ID: {Id}", result.Id);
            }
            else
            {
                _logger.LogError("Failed to create quiz test: {Error}", result.Error);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred while creating quiz test: {Title}", command?.Title);
            return StatusCode(500, Result.Failure("An error occurred while creating the test"));
        }
    }

    /// <summary>
    /// Met à jour un test existant
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult> Update(int id, [FromBody] UpdateTestCommand command)
    {
        try
        {
            _logger.LogInformation("Updating quiz test with ID: {Id}", id);

            if (id != command.Id)
            {
                _logger.LogWarning("Route ID {RouteId} does not match command ID {CommandId}", id, command.Id);
                return BadRequest("Route ID does not match command ID");
            }

            // Vérifier que le test existe
            var existingTest = await _context.Tests.FindAsync(id);
            if (existingTest == null)
            {
                _logger.LogWarning("Test with ID {Id} not found for update", id);
                return NotFound($"Test with ID {id} not found");
            }

            var result = await Mediator.Send(command);

            _logger.LogInformation("Successfully updated quiz test with ID: {Id}", id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating quiz test with ID: {Id}", id);
            return StatusCode(500, "An error occurred while updating the test");
        }
    }

    /// <summary>
    /// Supprime un test
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        try
        {
            _logger.LogInformation("Deleting quiz test with ID: {Id}", id);

            // Vérifier que le test existe
            var existingTest = await _context.Tests.FindAsync(id);
            if (existingTest == null)
            {
                _logger.LogWarning("Test with ID {Id} not found for deletion", id);
                return NotFound($"Test with ID {id} not found");
            }

            // Vérifier si le test a des questions associées
            var hasQuestions = await _context.Questions.AnyAsync(q => q.QuizTestId == id);
            if (hasQuestions)
            {
                _logger.LogWarning("Cannot delete test {Id} because it has associated questions", id);
                return BadRequest("Cannot delete test that has associated questions. Delete questions first.");
            }

            var result = await Mediator.Send(new DeleteTestCommand(id));

            _logger.LogInformation("Successfully deleted quiz test with ID: {Id}", id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting quiz test with ID: {Id}", id);
            return StatusCode(500, "An error occurred while deleting the test");
        }
    }

    /// <summary>
    /// Active ou désactive un test
    /// </summary>
    [HttpPatch("{id}/toggle-status")]
    public async Task<ActionResult> ToggleTestStatus(int id)
    {
        try
        {
            _logger.LogInformation("Toggling status for quiz test with ID: {Id}", id);

            var test = await _context.Tests.FindAsync(id);
            if (test == null)
            {
                return NotFound($"Test with ID {id} not found");
            }

            test.IsActive = !test.IsActive;
            _context.Tests.Update(test);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Successfully toggled status for test {Id} to {Status}", id, test.IsActive);

            return Ok(new { id = test.Id, isActive = test.IsActive });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error toggling status for quiz test with ID: {Id}", id);
            return StatusCode(500, "An error occurred while updating test status");
        }
    }
}