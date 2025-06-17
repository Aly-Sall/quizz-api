using _Net6CleanArchitectureQuizzApp.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace _Net6CleanArchitectureQuizzApp.Infrastructure.Services;

public class TestInvitationService : ITestInvitationService
{
    private readonly IEmailService _emailService;
    private readonly ILogger<TestInvitationService> _logger;

    public TestInvitationService(
        IEmailService emailService,
        ILogger<TestInvitationService> logger)
    {
        _emailService = emailService;
        _logger = logger;
    }

    public async Task SendCandidateInvitationEmailAsync(string email, string? nom)
    {
        try
        {
            var subject = "Invitation au test de recrutement";
            var body = $"Bonjour {nom ?? "Candidat"},\n\nVous avez été invité à passer un test...";

            await _emailService.SendEmailAsync(email, subject, body);
            _logger.LogInformation("Invitation email sent to {Email}", email);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send invitation email to {Email}", email);
            throw;
        }
    }

    public async Task<bool> ValidateInvitationTokenAsync(string token)
    {
        // Implémentation de validation du token
        return await Task.FromResult(true);
    }

    public async Task<string> GenerateInvitationTokenAsync(string email)
    {
        // Génération d'un token d'invitation
        return await Task.FromResult(Guid.NewGuid().ToString());
    }
}
