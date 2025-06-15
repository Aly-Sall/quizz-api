using _Net6CleanArchitectureQuizzApp.Application.Common.Interfaces;

namespace _Net6CleanArchitectureQuizzApp.Infrastructure.Services;

public class EmailService : IEmailService
{
    public async Task<bool> SendTestInvitationAsync(string email, string candidateName, string testTitle, string invitationLink, CancellationToken cancellationToken = default)
    {
        // Pour le moment, on simule l'envoi d'email
        await Task.Delay(100, cancellationToken);

        Console.WriteLine($"📧 Email d'invitation envoyé à {email}");
        Console.WriteLine($"   Candidat: {candidateName}");
        Console.WriteLine($"   Test: {testTitle}");
        Console.WriteLine($"   Lien: {invitationLink}");

        return true;
    }

    public async Task<bool> SendEmailAsync(string to, string subject, string body, CancellationToken cancellationToken = default)
    {
        await Task.Delay(100, cancellationToken);
        Console.WriteLine($"📧 Email envoyé à {to}: {subject}");
        return true;
    }
}