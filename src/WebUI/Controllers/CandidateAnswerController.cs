using _Net6CleanArchitectureQuizzApp.Application.Common.Models;
using _Net6CleanArchitectureQuizzApp.Application.ResponseDev.Commands.CreateResponseDev;
using _Net6CleanArchitectureQuizzApp.Application.TentativeDev.Commands.CreateTentative;
using Microsoft.AspNetCore.Mvc;

namespace _Net6CleanArchitectureQuizzApp.WebUI.Controllers;

public class CandidateAnswerController : ApiControllerBase
{

    [HttpPost]
    public async Task<Result> Create(CreateResponseCommand command)
    {
        return await Mediator.Send(command);
    }
}
