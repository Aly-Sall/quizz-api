// src/Infrastructure/Services/ApplicationEmailService.cs
using _Net6CleanArchitectureQuizzApp.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace _Net6CleanArchitectureQuizzApp.Infrastructure.Services;

/// <summary>
/// Implémentation du service d'email pour l'application
/// Nom modifié pour éviter les conflits avec d'autres EmailService
/// </summary>
public class ApplicationEmailService : IEmailService
{
    private readonly ILogger<ApplicationEmailService> _logger;

    public ApplicationEmailService(ILogger<ApplicationEmailService> logger)
    {
        _logger = logger;
    }

    public async Task<bool> SendTestInvitationAsync(
        string recipientEmail,
        string candidateName,
        string testTitle,
        string invitationLink,
        CancellationToken cancellationToken = default)
    {
        // TODO: Implémenter l'envoi d'email réel avec SendGrid, SMTP, etc.
        _logger.LogInformation("📧 SIMULATION: Sending test invitation email");
        _logger.LogInformation("📧 To: {Email}", recipientEmail);
        _logger.LogInformation("📧 Candidate: {Name}", candidateName);
        _logger.LogInformation("📧 Test: {Title}", testTitle);
        _logger.LogInformation("📧 Link: {Link}", invitationLink);

        // Simuler un délai d'envoi
        await Task.Delay(100, cancellationToken);

        // Simuler le succès pour le développement
        _logger.LogInformation("✅ Email invitation sent successfully (simulated)");
        return true;
    }

    public async Task<bool> SendEmailAsync(
        string to,
        string subject,
        string body,
        CancellationToken cancellationToken = default)
    {
        // TODO: Implémenter l'envoi d'email réel
        _logger.LogInformation("📧 SIMULATION: Sending email");
        _logger.LogInformation("📧 To: {To}", to);
        _logger.LogInformation("📧 Subject: {Subject}", subject);
        _logger.LogInformation("📧 Body length: {Length} characters", body.Length);

        await Task.Delay(100, cancellationToken);

        _logger.LogInformation("✅ Email sent successfully (simulated)");
        return true;
    }
}