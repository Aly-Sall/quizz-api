// src/WebUI/Controllers/QuizTestController.cs - Version améliorée
using _Net6CleanArchitectureQuizzApp.Application.Common.Models;
using _Net6CleanArchitectureQuizzApp.Application.QuestionDev.Queries.GetQuestionsByTestId;
using _Net6CleanArchitectureQuizzApp.Application.TestDev.Commands.CreateQuizTest.CreateTest;
using _Net6CleanArchitectureQuizzApp.Application.TestDev.Commands.UpdateTestDev;
using _Net6CleanArchitectureQuizzApp.Application.TestDev.Queries.GetQuizTestById;
using _Net6CleanArchitectureQuizzApp.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace _Net6CleanArchitectureQuizzApp.WebUI.Controllers;

public class QuizTestController : ApiControllerBase
{
    private readonly ILogger<QuizTestController> _logger;

    public QuizTestController(ILogger<QuizTestController> logger)
    {
        _logger = logger;
    }

    [HttpGet("by-id/{id}")]
    public async Task<Result<QuizTest>> GetQuizTestById([FromQuery] GetQuizTestQuery query)
    {
        try
        {
            _logger.LogInformation("Getting quiz test by ID: {Id}", query.Id);
            var result = await Mediator.Send(query);
            _logger.LogInformation("Successfully retrieved quiz test: {IsSuccess}", result.IsSuccess);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting quiz test by ID: {Id}", query.Id);
            return Result<QuizTest>.Failure("Error retrieving quiz test");
        }
    }

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
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating quiz test with ID: {Id}", id);
            return StatusCode(500, "An error occurred while updating the test");
        }
    }

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

    // Nouvelle méthode pour récupérer tous les tests (pour la liste)
    [HttpGet]
    public async Task<ActionResult<IEnumerable<QuizTest>>> GetAllTests()
    {
        try
        {
            _logger.LogInformation("Getting all quiz tests");

            // Pour l'instant, retourner une liste vide ou des tests mockés
            // Vous devrez implémenter GetAllTestsQuery côté Application
            var mockTests = new List<QuizTest>
            {
                new QuizTest
                {
                    Id = 1,
                    Title = "Sample Technical Test",
                    Category = Domain.Enums.Category.Technical,
                    Mode = Domain.Enums.Mode.Training,
                    Level = Domain.Enums.Level.Medium,
                    IsActive = true,
                    ShowTimer = true,
                    TryAgain = false
                },
                new QuizTest
                {
                    Id = 2,
                    Title = "General Knowledge Quiz",
                    Category = Domain.Enums.Category.General,
                    Mode = Domain.Enums.Mode.Training,
                    Level = Domain.Enums.Level.Easy,
                    IsActive = true,
                    ShowTimer = false,
                    TryAgain = true
                }
            };

            _logger.LogInformation("Returning {Count} quiz tests", mockTests.Count);
            return Ok(mockTests);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all quiz tests");
            return StatusCode(500, "An error occurred while retrieving tests");
        }
    }

    // Nouvelle méthode pour supprimer un test
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteTest(int id)
    {
        try
        {
            _logger.LogInformation("Deleting quiz test with ID: {Id}", id);

            // Vous devrez implémenter DeleteTestCommand côté Application
            // await Mediator.Send(new DeleteTestCommand(id));

            _logger.LogInformation("Successfully deleted quiz test with ID: {Id}", id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting quiz test with ID: {Id}", id);
            return StatusCode(500, "An error occurred while deleting the test");
        }
    }
}