using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _Net6CleanArchitectureQuizzApp.Application.Common.Models;
using _Net6CleanArchitectureQuizzApp.Application.TestDev.Commands.CreateQuizTest.CreateTest;
using _Net6CleanArchitectureQuizzApp.Domain.Entities;
using _Net6CleanArchitectureQuizzApp.Domain.Enums;
using FluentValidation;
using MediatR;

namespace _Net6CleanArchitectureQuizzApp.Application.QuestionDev.Commands.CreateQuestionDev;

public class CreateQuestionCommand : IRequest<Result>
{
    public string Content { get; set; } = null!;
    public QuestionType Type { get; set; }
    public string? AnswerDetails { get; set; }
    public int QuizTestId { get; set; }
    // CORRIGÉ: Définir avec valeur par défaut pour éviter les erreurs null
    public string ListOfCorrectAnswerIds { get; set; } = "[]";
    // CORRIGÉ: S'assurer que la liste est initialisée
    public List<QuestionChoice> Choices { get; set; } = new List<QuestionChoice>();
}

public class CreateQuestionCommandValidator : AbstractValidator<CreateQuestionCommand>
{
    public CreateQuestionCommandValidator()
    {
        RuleFor(v => v.Content)
            .NotEmpty()
            .WithMessage("Le contenu de la question est requis")
            .MinimumLength(10)
            .WithMessage("Le contenu doit contenir au moins 10 caractères")
            .MaximumLength(1000)
            .WithMessage("Le contenu ne peut pas dépasser 1000 caractères");

        RuleFor(v => v.QuizTestId)
            .NotNull()
            .WithMessage("L'ID du test est requis")
            .GreaterThan(0)
            .WithMessage("L'ID du test doit être supérieur à 0");

        RuleFor(v => v.ListOfCorrectAnswerIds)
            .NotEmpty()
            .WithMessage("Au moins une réponse correcte doit être spécifiée");

        RuleFor(v => v.Choices)
            .NotNull()
            .WithMessage("Les choix de réponses sont requis")
            .Must(choices => choices != null && choices.Count >= 2)
            .WithMessage("Au moins 2 choix de réponses sont requis");

        RuleFor(v => v.Type)
            .IsInEnum()
            .WithMessage("Le type de question doit être valide");
    }
}