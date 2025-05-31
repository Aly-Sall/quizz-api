// src/Application/TestInvitation/Commands/SendTestInvitation/SendTestInvitationCommand.cs
using _Net6CleanArchitectureQuizzApp.Application.Common.Models;
using FluentValidation;
using MediatR;

namespace _Net6CleanArchitectureQuizzApp.Application.TestInvitation.Commands.SendTestInvitation;

public class SendTestInvitationCommand : IRequest<Result>
{
    public string CandidateEmail { get; set; } = null!;
    public string CandidateName { get; set; } = null!;
    public int TestId { get; set; }
    public int ExpirationHours { get; set; } = 72; // 3 jours par défaut
}

public class SendTestInvitationCommandValidator : AbstractValidator<SendTestInvitationCommand>
{
    public SendTestInvitationCommandValidator()
    {
        RuleFor(v => v.CandidateEmail)
            .NotEmpty()
            .EmailAddress()
            .WithMessage("Une adresse email valide est requise");

        RuleFor(v => v.CandidateName)
            .NotEmpty()
            .WithMessage("Le nom du candidat est requis");

        RuleFor(v => v.TestId)
            .GreaterThan(0)
            .WithMessage("L'ID du test doit être valide");

        RuleFor(v => v.ExpirationHours)
            .InclusiveBetween(1, 168) // Entre 1 heure et 1 semaine
            .WithMessage("La durée d'expiration doit être entre 1 et 168 heures");
    }
}