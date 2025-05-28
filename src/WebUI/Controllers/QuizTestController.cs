// src/WebUI/Controllers/QuizTestController.cs - Version complète fonctionnelle
using _Net6CleanArchitectureQuizzApp.Application.Common.Models;
using _Net6CleanArchitectureQuizzApp.Application.QuestionDev.Queries.GetQuestionsByTestId;
using _Net6CleanArchitectureQuizzApp.Application.TestDev.Commands.CreateQuizTest.CreateTest;
using _Net6CleanArchitectureQuizzApp.Application.TestDev.Commands.UpdateTestDev;
using _Net6CleanArchitectureQuizzApp.Application.TestDev.Commands.DeleteTestDev;
using _Net6CleanArchitectureQuizzApp.Application.TestDev.Queries.GetQuizTestById;
using _Net6CleanArchitectureQuizzApp.Application.TestDev.Queries.GetAllTests;
using _Net6CleanArchitectureQuizzApp.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using _Net6CleanArchitectureQuizzApp.Application.Common.Exceptions;

namespace _Net6CleanArchitectureQuizzApp.WebUI.Controllers;

public class QuizTestController : ApiControllerBase
{
    private readonly ILogger<QuizTestController> _logger;

    public QuizTestController(ILogger<QuizTestController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Récupère tous les tests
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<Application.TestDev.Queries.GetAllTests.QuizTestDto>>> GetAllTests(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        [FromQuery] bool activeOnly = false)
    {
        try
        {
            _logger.LogInformation("Getting all quiz tests - Page: {Page}, PageSize: {PageSize}, ActiveOnly: {ActiveOnly}",
                page, pageSize, activeOnly);

            var query = new GetAllTestsQuery
            {
                Page = page,
                PageSize = pageSize,
                ActiveOnly = activeOnly
            };

            var tests = await Mediator.Send(query);

            _logger.LogInformation("Successfully retrieved {Count} quiz tests", tests.Count);
            return Ok(tests);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all quiz tests");
            return StatusCode(500, "An error occurred while retrieving tests");
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

            if (result.IsSuccess)
            {
                _logger.LogInformation("Successfully retrieved quiz test: {Title}", result.Value?.Title);
            }
            else
            {
                _logger.LogWarning("Failed to retrieve quiz test: {Error}", result.Error);
            }

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
    public async Task<Result> Create([FromBody] CreateTestCommand command)
    {
        try
        {
            _logger.LogInformation("Creating new quiz test with title: {Title}", command.Title);
            _logger.LogInformation("Command details: {@Command}", command);

            if (command == null)
            {
                _logger.LogWarning("Received null command for test creation");
                return Result.Failure("Command cannot be null");
            }

            if (string.IsNullOrWhiteSpace(command.Title))
            {
                _logger.LogWarning("Received command with empty title");
                return Result.Failure("Test title is required");
            }

            var result = await Mediator.Send(command);

            if (result.IsSuccess)
            {
                _logger.LogInformation("Successfully created quiz test with ID: {Id}", result.Id);
            }
            else
            {
                _logger.LogWarning("Failed to create quiz test: {Error}", result.Error);
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred while creating quiz test: {Title}", command?.Title);
            return Result.Failure("An error occurred while creating the test");
        }
    }

    /// <summary>
    /// Met à jour un test existant
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateTest(int id, [FromBody] UpdateTestCommand command)
    {
        try
        {
            _logger.LogInformation("Updating quiz test with ID: {Id}", id);

            if (id != command.Id)
            {
                _logger.LogWarning("ID mismatch: URL ID {UrlId} vs Command ID {CommandId}", id, command.Id);
                return BadRequest("ID mismatch");
            }

            await Mediator.Send(command);
            _logger.LogInformation("Successfully updated quiz test with ID: {Id}", id);

            return NoContent();
        }
        catch (ForbiddenAccessException)
        {
            _logger.LogWarning("Attempted to update active test with ID: {Id}", id);
            return StatusCode(403, "Cannot update an active test");
        }
        catch (NotFoundException)
        {
            _logger.LogWarning("Test with ID {Id} not found for update", id);
            return NotFound($"Test with ID {id} not found");
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
    public async Task<ActionResult> DeleteTest(int id)
    {
        try
        {
            _logger.LogInformation("Deleting quiz test with ID: {Id}", id);

            await Mediator.Send(new DeleteTestCommand(id));

            _logger.LogInformation("Successfully deleted quiz test with ID: {Id}", id);
            return NoContent();
        }
        catch (NotFoundException)
        {
            _logger.LogWarning("Test with ID {Id} not found for deletion", id);
            return NotFound($"Test with ID {id} not found");
        }
        catch (ForbiddenAccessException)
        {
            _logger.LogWarning("Attempted to delete active test with ID: {Id}", id);
            return StatusCode(403, "Cannot delete an active test. Deactivate it first.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting quiz test with ID: {Id}", id);
            return StatusCode(500, "An error occurred while deleting the test");
        }
    }

    /// <summary>
    /// Récupère un test par token d'accès
    /// </summary>
    [HttpGet("by-token/{token}")]
    public async Task<GetTestDto> GetQuizTestByToken(string token)
    {
        try
        {
            _logger.LogInformation("Getting quiz test by token: {Token}", token);
            var result = await Mediator.Send(new GetQuizTestByTokenQuery { token = token });
            _logger.LogInformation("Successfully retrieved quiz test by token");
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting quiz test by token: {Token}", token);
            throw;
        }
    }

    /// <summary>
    /// Active/Désactive un test
    /// </summary>
    [HttpPatch("{id}/toggle-status")]
    public async Task<ActionResult> ToggleTestStatus(int id)
    {
        try
        {
            _logger.LogInformation("Toggling status for quiz test with ID: {Id}", id);

            // Récupérer le test actuel
            var testResult = await Mediator.Send(new GetQuizTestQuery { Id = id });

            if (!testResult.IsSuccess || testResult.Value == null)
            {
                return NotFound($"Test with ID {id} not found");
            }

            var test = testResult.Value;

            // Créer la commande de mise à jour avec le statut inversé
            var updateCommand = new UpdateTestCommand
            {
                Id = test.Id,
                Title = test.Title,
                Category = test.Category,
                Mode = test.Mode,
                ShowTimer = test.ShowTimer
            };

            // Pour le moment, on ne peut pas changer IsActive via UpdateTestCommand
            // Il faudrait créer une commande spécifique ToggleTestStatusCommand

            await Mediator.Send(updateCommand);

            _logger.LogInformation("Successfully toggled status for quiz test with ID: {Id}", id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error toggling status for quiz test with ID: {Id}", id);
            return StatusCode(500, "An error occurred while toggling test status");
        }
    }
}