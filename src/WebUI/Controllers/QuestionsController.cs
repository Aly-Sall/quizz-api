using _Net6CleanArchitectureQuizzApp.Application.Common.Models;
using _Net6CleanArchitectureQuizzApp.Application.QuestionDev.Commands.CreateQuestionDev;
using _Net6CleanArchitectureQuizzApp.Application.QuestionDev.Commands.DeleteQuestionDev;
using _Net6CleanArchitectureQuizzApp.Application.QuestionDev.Queries.GetQuestionsByTestId;
using _Net6CleanArchitectureQuizzApp.Application.TestDev.Commands.CreateQuizTest.CreateTest;
using _Net6CleanArchitectureQuizzApp.Application.TestDev.Commands.UpdateTestDev;
using _Net6CleanArchitectureQuizzApp.Application.TestDev.Queries.GetQuizTestById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace _Net6CleanArchitectureQuizzApp.WebUI.Controllers;

public class QuestionsController : ApiControllerBase
{
    [HttpGet("GetQuestionsByTestId")]
    public async Task <List<GetQuestionDto>> GetQuestionsByTestId([FromQuery] GetQuestionQuery query)
    {
        return await Mediator.Send(query);
    }

    [HttpPost]
    public async Task<Result> Create([FromBody] CreateQuestionCommand command)
    {
        return await Mediator.Send(command);
    }
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        await Mediator.Send(new DeleteQuestionCommand(id));

        return NoContent();
    }
    [HttpPost("GenerateQuestionsUsingOpenAI")]
    public void CreateQuestions()
    {
    }

}