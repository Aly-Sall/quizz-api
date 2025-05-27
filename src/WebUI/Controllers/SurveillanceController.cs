using _Net6CleanArchitectureQuizzApp.Application.Common.Models;
using _Net6CleanArchitectureQuizzApp.Application.QuestionDev.Queries.GetQuestionsByTestId;
using _Net6CleanArchitectureQuizzApp.Application.SurveillanceDev.Commands.CreateSurveillanceDev;
using _Net6CleanArchitectureQuizzApp.Application.SurveillanceDev.Queries.GetSurveillanceByTentative;
using Microsoft.AspNetCore.Mvc;

namespace _Net6CleanArchitectureQuizzApp.WebUI.Controllers;

public class SurveillanceController : ApiControllerBase
{
    [HttpPost]
    public async Task<Result> CreateSurveiilance(CreateSurveillanceCommand command)
    {
        return await Mediator.Send(command);
    }
    [HttpGet("GetSurveiilancesByTentative")]
    public async Task<List<GetSurveillanceDto>> GetSurveillancesByTentative([FromQuery] GetSurveillanceQuery query)
    {
        return await Mediator.Send(query);
    }

}
