// src/Infrastructure/Services/CandidateInvitationService.cs - FICHIER CORRIGÉ
using _Net6CleanArchitectureQuizzApp.Application.Common.Interfaces;
using _Net6CleanArchitectureQuizzApp.Application.Account.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace _Net6CleanArchitectureQuizzApp.Infrastructure.Services;

public class CandidateInvitationService : ICandidateInvitationService
{
    private readonly IEmailService _emailService;
    private readonly IApplicationDbContext _context;
    private readonly ILogger<CandidateInvitationService> _logger;
    private readonly IConfiguration _configuration;

    public CandidateInvitationService(
        IEmailService emailService,
        IApplicationDbContext context,
        ILogger<CandidateInvitationService> logger,
        IConfiguration configuration)
    {
        _emailService = emailService;
        _context = context;
        _logger = logger;
        _configuration = configuration;
    }

    public async Task<bool> SendCandidateInvitationEmailAsync(string candidateEmail, string candidateName)
    {
        try
        {
            _logger.LogInformation("🔍 Envoi d'invitation candidat à {Email}", candidateEmail);

            // ✅ LIEN VERS LOGIN AVEC PARAMÈTRES CANDIDAT
            var frontendUrl = _configuration["Frontend:BaseUrl"] ?? "http://localhost:4200";
            var invitationLink = $"{frontendUrl}/login?role=candidate&email={candidateEmail}";

            // Contenu de l'email corrigé
            var subject = "Invitation à passer vos tests - Quiz App";
            var body = $@"
<html>
<body style=""font-family: Arial, sans-serif; line-height: 1.6; color: #333;"">
    <div style=""max-width: 600px; margin: 0 auto; padding: 20px;"">
        <h2 style=""color: #2c3e50; border-bottom: 3px solid #3498db; padding-bottom: 10px;"">
            📋 Vos Tests Disponibles - Quiz App
        </h2>
        
        <p>Bonjour <strong>{candidateName}</strong>,</p>
        
        <p>Vos tests de candidature sont maintenant prêts ! Connectez-vous pour accéder à votre espace candidat.</p>
        
        <div style=""background-color: #f8f9fa; padding: 15px; border-radius: 5px; margin: 20px 0;"">
            <h3 style=""margin: 0; color: #2c3e50;"">📝 Instructions simples :</h3>
            <ol style=""margin: 10px 0;"">
                <li>Cliquez sur le bouton ci-dessous</li>
                <li>Sélectionnez <strong>Candidat</strong> comme type de compte</li>
                <li>Entrez vos identifiants (email: {candidateEmail})</li>
                <li>Accédez à vos tests</li>
            </ol>
        </div>
        
        <div style=""text-align: center; margin: 30px 0;"">
            <a href=""{invitationLink}"" 
               style=""background-color: #28a745; color: white; padding: 15px 40px; 
                      text-decoration: none; border-radius: 8px; font-weight: bold;
                      display: inline-block; font-size: 16px;"">
                📚 Accéder à Mes Tests
            </a>
        </div>
        
        <div style=""background-color: #d1ecf1; padding: 10px; border-radius: 5px; margin: 20px 0;"">
            <p style=""margin: 0; color: #0c5460;"">
                <strong>👤 Important :</strong> Utilisez vos identifiants candidat pour vous connecter
            </p>
        </div>
        
        <p><small style=""color: #7f8c8d;"">
            Si le bouton ne fonctionne pas, copiez et collez ce lien dans votre navigateur :<br/>
            <a href=""{invitationLink}"">{invitationLink}</a>
        </small></p>
        
        <hr style=""border: none; border-top: 1px solid #eee; margin: 30px 0;""/>
        <p style=""color: #7f8c8d; font-size: 12px;"">
            Une fois connecté, vous pourrez voir tous les tests disponibles et choisir celui que vous souhaitez passer.<br/><br/>
            Cordialement,<br/>
            <strong>L'équipe Quiz App</strong>
        </p>
    </div>
</body>
</html>";

            // Envoyer l'email
            var emailSent = await _emailService.SendEmailAsync(candidateEmail, subject, body);

            if (emailSent)
            {
                _logger.LogInformation("✅ Email d'invitation envoyé avec succès à {Email}", candidateEmail);
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
            _logger.LogError(ex, "❌ Erreur lors de l'envoi de l'email d'invitation à {Email}", candidateEmail);
            return false;
        }
    }

    public async Task<List<TestDto>> GetAvailableTestsForCandidateAsync(string candidateEmail)
    {
        // TODO: Implémenter la récupération des tests disponibles
        return new List<TestDto>();
    }
}