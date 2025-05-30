// src/WebUI/Controllers/QuestionsController.cs - Version complète
using _Net6CleanArchitectureQuizzApp.Application.Common.Interfaces;
using _Net6CleanArchitectureQuizzApp.Application.Common.Models;
using _Net6CleanArchitectureQuizzApp.Application.QuestionDev.Commands.CreateQuestionDev;
using _Net6CleanArchitectureQuizzApp.Application.QuestionDev.Commands.DeleteQuestionDev;
using _Net6CleanArchitectureQuizzApp.Application.QuestionDev.Queries.GetQuestionsByTestId;
using _Net6CleanArchitectureQuizzApp.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace _Net6CleanArchitectureQuizzApp.WebUI.Controllers;

public class QuestionsController : ApiControllerBase
{
    private readonly ILogger<QuestionsController> _logger;
    private readonly IApplicationDbContext _context;

    public QuestionsController(ILogger<QuestionsController> logger, IApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    /// <summary>
    /// Récupère toutes les questions (pour la fonctionnalité d'assignation)
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<GetQuestionDto>>> GetAllQuestions()
    {
        try
        {
            _logger.LogInformation("Getting all questions from database");

            var questions = await _context.Questions
                .OrderByDescending(q => q.Id)
                .Select(q => new GetQuestionDto
                {
                    Id = q.Id,
                    Content = q.Content,
                    Type = q.Type,
                    AnswerDetails = q.AnswerDetails,
                    QuizTestId = q.QuizTestId,
                    Choices = q.Choices ?? new QuestionChoice[0],
                    ListOfCorrectAnswerIds = q.ListOfCorrectAnswerIds
                })
                .ToListAsync();

            _logger.LogInformation("Retrieved {Count} questions from database", questions.Count);

            return Ok(questions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all questions");
            return StatusCode(500, "An error occurred while retrieving questions");
        }
    }

    /// <summary>
    /// Récupère les questions d'un test spécifique
    /// </summary>
    [HttpGet("GetQuestionsByTestId")]
    public async Task<List<GetQuestionDto>> GetQuestionsByTestId([FromQuery] GetQuestionQuery query)
    {
        _logger.LogInformation("Getting questions for test ID: {TestId}", query.Id);
        return await Mediator.Send(query);
    }


    /// <summary>
    /// Crée une nouvelle question
    /// </summary>
    [HttpPost]
    public async Task<Result> Create([FromBody] CreateQuestionCommand command)
    {
        try
        {
            //_logger.LogInformation("Creating new question for test ID: {TestId}", command.QuizTestId);
            return await Mediator.Send(command);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating question");
            return Result.Failure("An error occurred while creating the question");
        }
    }


    /// <summary
    /// Supprime une question
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        try
        {
            _logger.LogInformation("Deleting question with ID: {Id}", id);
            await Mediator.Send(new DeleteQuestionCommand(id));
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting question with ID: {Id}", id);
            return StatusCode(500, "An error occurred while deleting the question");
        }
    }

    /// <summary>
    /// Assigne une question existante à un test
    /// </summary>
    [HttpPost("{questionId}/assign-to-test/{testId}")]
    public async Task<ActionResult<Result>> AssignQuestionToTest(int questionId, int testId)
    {
        try
        {
            _logger.LogInformation("Assigning question {QuestionId} to test {TestId}", questionId, testId);

            // Vérifier que la question existe
            var question = await _context.Questions.FindAsync(questionId);
            if (question == null)
            {
                return NotFound(Result.Failure($"Question with ID {questionId} not found"));
            }

            // Vérifier que le test existe
            var test = await _context.Tests.FindAsync(testId);
            if (test == null)
            {
                return NotFound(Result.Failure($"Test with ID {testId} not found"));
            }

            // Vérifier que la question n'est pas déjà assignée à ce test
            if (question.QuizTestId == testId)
            {
                return BadRequest(Result.Failure("Question is already assigned to this test"));
            }

            // Assigner la question au test
            question.QuizTestId = testId;
            _context.Questions.Update(question);

            var result = await _context.SaveChangesAsync();

            if (result > 0)
            {
                _logger.LogInformation("Successfully assigned question {QuestionId} to test {TestId}", questionId, testId);
                return Ok(Result.Success());
            }
            else
            {
                return StatusCode(500, Result.Failure("Failed to assign question to test"));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error assigning question {QuestionId} to test {TestId}", questionId, testId);
            return StatusCode(500, Result.Failure("An error occurred while assigning the question"));
        }
    }

    /// <summary>
    /// Génère des questions avec OpenAI (si implémenté)
    /// </summary>
    [HttpPost("GenerateQuestionsUsingOpenAI")]
    public async Task<ActionResult<Result>> GenerateQuestionsUsingOpenAI([FromBody] GenerateQuestionsRequest request)
    {
        try
        {
            _logger.LogInformation("Generating questions using OpenAI for topic: {Topic}", request.Topic);

            // TODO: Implémenter la génération avec OpenAI
            return Ok(Result.Failure("OpenAI question generation not yet implemented"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating questions with OpenAI");
            return StatusCode(500, Result.Failure("An error occurred while generating questions"));
        }
    }
}

public class GenerateQuestionsRequest
{
    public string Topic { get; set; } = string.Empty;
    public int TestId { get; set; }
}