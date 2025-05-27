using _Net6CleanArchitectureQuizzApp.Application.Common.Models;
using _Net6CleanArchitectureQuizzApp.Application.QuestionDev.Commands.CreateQuestionDev;
using _Net6CleanArchitectureQuizzApp.Application.TentativeDev.Commands.CreateTentative;
using Microsoft.AspNetCore.Mvc;

namespace _Net6CleanArchitectureQuizzApp.WebUI.Controllers;

public class TentativeController : ApiControllerBase
{

    [HttpPost]
    public async Task<Result> Create(CreateTentativeCommand command)
    {
        return await Mediator.Send(command);
    }
}
