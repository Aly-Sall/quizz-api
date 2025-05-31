// src/Application/TestInvitation/Commands/SendTestInvitation/SendTestInvitationHandler.cs
using _Net6CleanArchitectureQuizzApp.Application.Common.Interfaces;
using _Net6CleanArchitectureQuizzApp.Application.Common.Models;
using _Net6CleanArchitectureQuizzApp.Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace _Net6CleanArchitectureQuizzApp.Application.TestInvitation.Commands.SendTestInvitation;

public class SendTestInvitationHandler : IRequestHandler<SendTestInvitationCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly IEmailService _emailService;
    private readonly IValidator<SendTestInvitationCommand> _validator;
    private readonly IConfiguration _configuration;

    public SendTestInvitationHandler(
        IApplicationDbContext context,
        IEmailService emailService,
        IValidator<SendTestInvitationCommand> validator,
        IConfiguration configuration)
    {
        _context = context;
        _emailService = emailService;
        _validator = validator;
        _configuration = configuration;
    }

    public async Task<Result> Handle(SendTestInvitationCommand request, CancellationToken cancellationToken)
    {
        // Validation
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result.Failure(string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));
        }

        try
        {
            // Vérifier que le test existe
            var test = await _context.Tests.FindAsync(request.TestId);
            if (test == null)
            {
                return Result.Failure($"Test avec l'ID {request.TestId} introuvable");
            }

            // Vérifier si une invitation récente existe déjà
            var existingToken = await _context.TestAccessTokens
                .Where(t => t.CandidateEmail == request.CandidateEmail
                        && t.TestId == request.TestId
                        && t.ExpirationTime > DateTime.UtcNow
                        && !t.IsUsed)
                .FirstOrDefaultAsync(cancellationToken);

            TestAccessToken token;

            if (existingToken != null)
            {
                // Utiliser le token existant
                token = existingToken;
            }
            else
            {
                // Créer un nouveau token d'accès
                token = new TestAccessToken
                {
                    CandidateEmail = request.CandidateEmail,
                    TestId = request.TestId,
                    ExpirationTime = DateTime.UtcNow.AddHours(request.ExpirationHours),
                    IsUsed = false,
                    TentativeId = null
                };

                await _context.TestAccessTokens.AddAsync(token, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
            }

            // Générer le lien d'invitation
            var frontendUrl = _configuration["Frontend:BaseUrl"] ?? "http://localhost:4200";
            var invitationLink = $"{frontendUrl}/test-invitation/{token.Token}";

            // Envoyer l'email
            var emailSent = await _emailService.SendTestInvitationAsync(
                request.CandidateEmail,
                request.CandidateName,
                test.Title ?? "Test",
                invitationLink,
                cancellationToken);

            if (!emailSent)
            {
                return Result.Failure("Échec de l'envoi de l'email d'invitation");
            }

            return Result.Success(token.Id);
        }
        catch (Exception ex)
        {
            return Result.Failure("Une erreur s'est produite lors de l'envoi de l'invitation");
        }
    }
}