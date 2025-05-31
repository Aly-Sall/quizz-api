// src/Infrastructure/Services/EmailService.cs
using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using _Net6CleanArchitectureQuizzApp.Application.Common.Interfaces;

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

    public async Task<bool> SendTestInvitationAsync(string recipientEmail, string recipientName, string testTitle, string invitationLink, CancellationToken cancellationToken = default)
    {
        var subject = $"Invitation au test : {testTitle}";
        var body = GenerateInvitationEmailBody(recipientName, testTitle, invitationLink);

        return await SendEmailAsync(recipientEmail, subject, body, true, cancellationToken);
    }

    public async Task<bool> SendEmailAsync(string to, string subject, string body, bool isHtml = true, CancellationToken cancellationToken = default)
    {
        try
        {
            var smtpSettings = _configuration.GetSection("EmailSettings");

            using var client = new SmtpClient(smtpSettings["SmtpServer"])
            {
                Port = int.Parse(smtpSettings["Port"]),
                Credentials = new NetworkCredential(smtpSettings["Username"], smtpSettings["Password"]),
                EnableSsl = bool.Parse(smtpSettings["EnableSsl"])
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(smtpSettings["FromEmail"], smtpSettings["FromName"]),
                Subject = subject,
                Body = body,
                IsBodyHtml = isHtml
            };

            mailMessage.To.Add(to);

            await client.SendMailAsync(mailMessage, cancellationToken);

            _logger.LogInformation("Email sent successfully to {Email}", to);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {Email}", to);
            return false;
        }
    }

    private string GenerateInvitationEmailBody(string recipientName, string testTitle, string invitationLink)
    {
        return $@"
        <!DOCTYPE html>
        <html>
        <head>
            <meta charset='utf-8'>
            <title>Invitation au test</title>
            <style>
                body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
                .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                .header {{ background-color: #007bff; color: white; padding: 20px; text-align: center; }}
                .content {{ padding: 20px; background-color: #f9f9f9; }}
                .button {{ 
                    display: inline-block; 
                    padding: 12px 30px; 
                    background-color: #28a745; 
                    color: white; 
                    text-decoration: none; 
                    border-radius: 5px; 
                    margin: 20px 0;
                    font-weight: bold;
                }}
                .footer {{ padding: 20px; text-align: center; font-size: 12px; color: #666; }}
            </style>
        </head>
        <body>
            <div class='container'>
                <div class='header'>
                    <h1>Invitation au Test</h1>
                </div>
                <div class='content'>
                    <h2>Bonjour {recipientName},</h2>
                    <p>Vous êtes invité(e) à passer le test suivant :</p>
                    <h3>🎯 {testTitle}</h3>
                    <p>Cliquez sur le bouton ci-dessous pour commencer le test :</p>
                    <p style='text-align: center;'>
                        <a href='{invitationLink}' class='button'>Commencer le Test</a>
                    </p>
                    <p><strong>Important :</strong></p>
                    <ul>
                        <li>Ce lien est unique et personnel</li>
                        <li>Il expire après utilisation ou après 72 heures</li>
                        <li>Assurez-vous d'avoir une connexion internet stable</li>
                        <li>Prévoyez suffisamment de temps pour terminer le test</li>
                    </ul>
                    <p>Bonne chance !</p>
                </div>
                <div class='footer'>
                    <p>Si vous avez des questions, contactez l'administrateur.</p>
                    <p>Ce message a été envoyé automatiquement, merci de ne pas y répondre.</p>
                </div>
            </div>
        </body>
        </html>";
    }
}