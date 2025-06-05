/*using _Net6CleanArchitectureQuizzApp.Application.Common.Models;
using _Net6CleanArchitectureQuizzApp.Application.TestInvitation.Commands.SendTestInvitation;
using Microsoft.AspNetCore.Mvc;

namespace _Net6CleanArchitectureQuizzApp.WebUI.Controllers;

public class TestInvitationController : ApiControllerBase
{
    [HttpPost("send")]
    public async Task<ActionResult<Result>> SendInvitation([FromBody] SendTestInvitationCommand command)
    {
        try
        {
            var result = await Mediator.Send(command);

            if (result.IsSuccess)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, Result.Failure("Erreur interne du serveur"));
        }
    }

    [HttpPost("send-bulk")]
    public async Task<ActionResult<object>> SendBulkInvitations([FromBody] SendBulkInvitationsRequest request)
    {
        var results = new List<object>();
        var successCount = 0;
        var failureCount = 0;

        foreach (var invitation in request.Invitations)
        {
            var command = new SendTestInvitationCommand
            {
                CandidateEmail = invitation.Email,
                CandidateName = invitation.Name,
                TestId = request.TestId,
                ExpirationHours = request.ExpirationHours
            };

            var result = await Mediator.Send(command);

            results.Add(new
            {
                email = invitation.Email,
                name = invitation.Name,
                success = result.IsSuccess,
                error = result.Error
            });

            if (result.IsSuccess)
                successCount++;
            else
                failureCount++;
        }

        return Ok(new
        {
            totalSent = request.Invitations.Count,
            successCount,
            failureCount,
            results
        });
    }
}

public class SendBulkInvitationsRequest
{
    public int TestId { get; set; }
    public int ExpirationHours { get; set; } = 72;
    public List<CandidateInvitation> Invitations { get; set; } = new();
}

public class CandidateInvitation
{
    public string Email { get; set; } = null!;
    public string Name { get; set; } = null!;
}*/