// src/Infrastructure/Services/CandidateInvitationService.cs
using _Net6CleanArchitectureQuizzApp.Application.Common.Interfaces;
using _Net6CleanArchitectureQuizzApp.Application.Account.Models;
using Microsoft.Extensions.Logging;

namespace _Net6CleanArchitectureQuizzApp.Infrastructure.Services;

public class CandidateInvitationService : ICandidateInvitationService
{
    private readonly IEmailService _emailService;
    private readonly IApplicationDbContext _context;
    private readonly ILogger<CandidateInvitationService> _logger;

    public CandidateInvitationService(
        IEmailService emailService,
        IApplicationDbContext context,
        ILogger<CandidateInvitationService> logger)
    {
        _emailService = emailService;
        _context = context;
        _logger = logger;
    }

    public async Task<bool> SendCandidateInvitationEmailAsync(string candidateEmail, string candidateName)
    {
        try
        {
            _logger.LogInformation("🔍 Envoi d'invitation candidat à {Email}", candidateEmail);

            // Créer un lien d'invitation temporaire (pour le moment, un lien simple)
            var invitationToken = Guid.NewGuid().ToString();
            var invitationLink = $"https://your-app.com/candidate-access?token={invitationToken}";

            // Contenu de l'email
            var subject = "Invitation à passer un test - Quiz App";
            var body = $@"
                <h2>Bonjour {candidateName},</h2>
                <p>Vous avez été invité(e) à passer un test sur notre plateforme.</p>
                <p>Cliquez sur le lien ci-dessous pour accéder à vos tests disponibles :</p>
                <p><a href='{invitationLink}'>Accéder aux tests</a></p>
                <p>Cordialement,<br/>L'équipe Quiz App</p>
            ";

            // Envoyer l'email
            var emailSent = await _emailService.SendEmailAsync(candidateEmail, subject, body);

            if (emailSent)
            {
                _logger.LogInformation("✅ Email d'invitation envoyé avec succès à {Email}", candidateEmail);

                // TODO: Stocker le token d'invitation en base de données si nécessaire
                // await _context.CandidateInvitations.AddAsync(new CandidateInvitation 
                // { 
                //     Email = candidateEmail, 
                //     Token = invitationToken, 
                //     ExpiresAt = DateTime.UtcNow.AddDays(7) 
                // });
                // await _context.SaveChangesAsync();

                return true;
            }
            else
            {
                _logger.LogError("❌ Échec de l'envoi de l'email d'invitation à {Email}", candidateEmail);
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Erreur lors de l'envoi de l'invitation à {Email}", candidateEmail);
            return false;
        }
    }

    public async Task<List<TestDto>> GetAvailableTestsForCandidateAsync(string candidateEmail)
    {
        try
        {
            _logger.LogInformation("🔍 Récupération des tests disponibles pour {Email}", candidateEmail);

            // Pour le moment, retourner une liste vide ou des tests par défaut
            // TODO: Implémenter la logique métier pour récupérer les tests assignés au candidat

            var availableTests = new List<TestDto>();

            // Exemple de test par défaut (à remplacer par la vraie logique)
            // var tests = await _context.Tests
            //     .Where(t => t.IsActive)
            //     .Select(t => new TestDto 
            //     { 
            //         Id = t.Id, 
            //         Title = t.Title, 
            //         Description = t.Description,
            //         Duration = t.Duration
            //     })
            //     .ToListAsync();

            _logger.LogInformation("✅ {Count} tests disponibles pour {Email}", availableTests.Count, candidateEmail);

            return availableTests;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Erreur lors de la récupération des tests pour {Email}", candidateEmail);
            return new List<TestDto>();
        }
    }
}