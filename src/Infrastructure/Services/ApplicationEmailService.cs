// src/Infrastructure/Services/ApplicationEmailService.cs - Remplacer le fichier existant
using _Net6CleanArchitectureQuizzApp.Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Mail;

namespace _Net6CleanArchitectureQuizzApp.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<bool> SendTestInvitationAsync(string email, string candidateName, string testTitle, string invitationLink, CancellationToken cancellationToken = default)
    {
        var subject = $"Invitation au test : {testTitle}";
        var body = $@"
            <html>
            <body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>
                <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
                    <h2 style='color: #2c3e50; border-bottom: 3px solid #3498db; padding-bottom: 10px;'>
                        Invitation au Test - Quiz App
                    </h2>
                    
                    <p>Bonjour <strong>{candidateName}</strong>,</p>
                    
                    <p>Vous avez été invité(e) à passer le test suivant :</p>
                    <div style='background-color: #f8f9fa; padding: 15px; border-radius: 5px; margin: 20px 0;'>
                        <h3 style='margin: 0; color: #2c3e50;'>{testTitle}</h3>
                    </div>
                    
                    <p>Pour accéder au test, cliquez sur le bouton ci-dessous :</p>
                    <div style='text-align: center; margin: 30px 0;'>
                        <a href='{invitationLink}' 
                           style='background-color: #3498db; color: white; padding: 12px 30px; 
                                  text-decoration: none; border-radius: 5px; font-weight: bold;
                                  display: inline-block;'>
                            Accéder au Test
                        </a>
                    </div>
                    
                    <p><small style='color: #7f8c8d;'>
                        Si le bouton ne fonctionne pas, copiez et collez ce lien dans votre navigateur :<br/>
                        <a href='{invitationLink}'>{invitationLink}</a>
                    </small></p>
                    
                    <hr style='border: none; border-top: 1px solid #eee; margin: 30px 0;'/>
                    <p style='color: #7f8c8d; font-size: 12px;'>
                        Ce lien d'invitation est personnel et ne doit pas être partagé.<br/>
                        Cordialement,<br/>
                        L'équipe Quiz App
                    </p>
                </div>
            </body>
            </html>";

        return await SendEmailAsync(email, subject, body, cancellationToken);
    }

    public async Task<bool> SendEmailAsync(string to, string subject, string body, CancellationToken cancellationToken = default)
    {
        try
        {
            // Configuration SMTP depuis appsettings.json
            var smtpHost = _configuration["Email:SmtpHost"] ?? "smtp.gmail.com";
            var smtpPort = int.Parse(_configuration["Email:SmtpPort"] ?? "587");
            var smtpUsername = _configuration["Email:SmtpUsername"];
            var smtpPassword = _configuration["Email:SmtpPassword"];
            var fromAddress = _configuration["Email:FromAddress"];
            var fromName = _configuration["Email:FromName"] ?? "Quiz App";
            var enableSsl = bool.Parse(_configuration["Email:EnableSsl"] ?? "true");

            // Validation des paramètres requis
            if (string.IsNullOrEmpty(smtpUsername) || string.IsNullOrEmpty(smtpPassword) || string.IsNullOrEmpty(fromAddress))
            {
                _logger.LogError("❌ Configuration email manquante dans appsettings.json");

                // En mode développement, simuler l'envoi
                if (_configuration["ASPNETCORE_ENVIRONMENT"] == "Development")
                {
                    _logger.LogWarning("📧 MODE SIMULATION - Email qui serait envoyé :");
                    _logger.LogWarning("   De: {FromAddress}", fromAddress ?? "non-configuré");
                    _logger.LogWarning("   À: {To}", to);
                    _logger.LogWarning("   Sujet: {Subject}", subject);
                    _logger.LogWarning("   Corps: {Body}", string.IsNullOrEmpty(body) ? "vide" : body.Substring(0, Math.Min(100, body.Length)) + "...");
                    return true; // Simuler le succès en dev
                }

                return false;
            }

            _logger.LogInformation("📧 Envoi d'email à {To} via {SmtpHost}:{SmtpPort}", to, smtpHost, smtpPort);

            using var client = new SmtpClient(smtpHost, smtpPort);
            client.EnableSsl = enableSsl;
            client.UseDefaultCredentials = false;
            client.Credentials = new NetworkCredential(smtpUsername, smtpPassword);

            using var mailMessage = new MailMessage();
            mailMessage.From = new MailAddress(fromAddress, fromName);
            mailMessage.To.Add(to);
            mailMessage.Subject = subject;
            mailMessage.Body = body;
            mailMessage.IsBodyHtml = true;

            await client.SendMailAsync(mailMessage, cancellationToken);

            _logger.LogInformation("✅ Email envoyé avec succès à {To}", to);
            return true;
        }
        catch (SmtpException ex)
        {
            _logger.LogError(ex, "❌ Erreur SMTP lors de l'envoi à {To}: {Message}", to, ex.Message);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Erreur générale lors de l'envoi d'email à {To}", to);
            return false;
        }
    }
}