// src/Application/Common/Interfaces/IEmailService.cs
using Microsoft.Extensions.Logging;

namespace _Net6CleanArchitectureQuizzApp.Application.Common.Interfaces;

public interface IEmailService
{
    Task<bool> SendTestInvitationAsync(
        string recipientEmail,
        string candidateName,
        string testTitle,
        string invitationLink,
        CancellationToken cancellationToken = default);

    Task<bool> SendEmailAsync(
        string to,
        string subject,
        string body,
        CancellationToken cancellationToken = default);
}

