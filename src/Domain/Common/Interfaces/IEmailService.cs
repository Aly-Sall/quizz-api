// src/Application/Common/Interfaces/IEmailService.cs
namespace _Net6CleanArchitectureQuizzApp.Application.Common.Interfaces;

public interface IEmailService
{
    Task<bool> SendTestInvitationAsync(string recipientEmail, string recipientName, string testTitle, string invitationLink, CancellationToken cancellationToken = default);
    Task<bool> SendEmailAsync(string to, string subject, string body, bool isHtml = true, CancellationToken cancellationToken = default);
}