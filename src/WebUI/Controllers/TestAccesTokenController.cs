using System.Runtime.CompilerServices;
using _Net6CleanArchitectureQuizzApp.Application.Common.Models;
using _Net6CleanArchitectureQuizzApp.Application.QuestionDev.Queries.GetQuestionsByTestId;
using _Net6CleanArchitectureQuizzApp.Application.TestAccessTokenDev.Commands.GenerateTestAccessToken;
using _Net6CleanArchitectureQuizzApp.Application.TestAccessTokenDev.Commands.UpdateTestAccessToken;
using _Net6CleanArchitectureQuizzApp.Application.TestAccessTokenDev.Queries.GetTokenDetailsByToken;
using _Net6CleanArchitectureQuizzApp.Application.TestDev.Commands.UpdateTestDev;
using Microsoft.AspNetCore.Mvc;

namespace _Net6CleanArchitectureQuizzApp.WebUI.Controllers;

public class TestAccessTokenController : ApiControllerBase
{
    [HttpPost]
    public async Task<Result> GenerateAccessToken(GenerateTokenAccessCommand command)
    {
        return await Mediator.Send(command);

    }
    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateAccessToken(int id, UpdateTestAccessTokenCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest();
        }

        await Mediator.Send(command);


        return NoContent();
    }
    [HttpGet("{token}")]
    public async Task<GetTokenDto> GetTokenDetails(string token)
    {
        return await Mediator.Send(new GetTokenDetailsQuery { Token = token });
    }

}
