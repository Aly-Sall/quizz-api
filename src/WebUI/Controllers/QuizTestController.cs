using _Net6CleanArchitectureQuizzApp.Application.Common.Models;
using _Net6CleanArchitectureQuizzApp.Application.QuestionDev.Queries.GetQuestionsByTestId;
using _Net6CleanArchitectureQuizzApp.Application.TestDev.Commands.CreateQuizTest.CreateTest;
using _Net6CleanArchitectureQuizzApp.Application.TestDev.Commands.UpdateTestDev;
using _Net6CleanArchitectureQuizzApp.Application.TestDev.Queries.GetQuizTestById;
using _Net6CleanArchitectureQuizzApp.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace _Net6CleanArchitectureQuizzApp.WebUI.Controllers;


public class QuizTestController : ApiControllerBase
{
    [HttpGet("by-id/{id}")]
    public async Task<Result<QuizTest>> GetQuizTestById([FromQuery] GetQuizTestQuery query)
    {
        return await Mediator.Send(query);
    }

    [HttpPost]
    public async Task<Result> Create(CreateTestCommand command)
    {
        return await Mediator.Send(command);
    }
    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateTest(int id,UpdateTestCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest();
        }

        await Mediator.Send(command);

        return NoContent();
    }
    [HttpGet("by-token/{token}")]
    public async Task<GetTestDto> GetQuizTestByToken(string token)
    {
        return await Mediator.Send(new GetQuizTestByTokenQuery { token = token });
    }
}

