// src/WebUI/Controllers/QuizTestController.cs - Version temporaire simplifiée
using _Net6CleanArchitectureQuizzApp.Application.Common.Models;
using _Net6CleanArchitectureQuizzApp.Application.TestDev.Commands.CreateQuizTest.CreateTest;
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

    /// <summary>
    /// Test simple - Récupère tous les tests
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<object>>> GetAllTests()
    {
        try
        {
            _logger.LogInformation("Getting all quiz tests - simple version");

            // Version ultra-simple pour tester
            var mockTests = new List<object>
            {
                new { Id = 1, Title = "Test Sample 1", IsActive = true },
                new { Id = 2, Title = "Test Sample 2", IsActive = false }
            };

            return Ok(mockTests);
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

            if (command == null)
            {
                return Result.Failure("Command cannot be null");
            }

            if (string.IsNullOrWhiteSpace(command.Title))
            {
                return Result.Failure("Test title is required");
            }

            var result = await Mediator.Send(command);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred while creating quiz test: {Title}", command?.Title);
            return Result.Failure("An error occurred while creating the test");
        }
    }
}