// src/Application/QuestionDev/Queries/GetQuestionsByTestId/GetQuestionDto.cs - VERSION CORRIGÉE
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _Net6CleanArchitectureQuizzApp.Application.Common.Mappings;
using _Net6CleanArchitectureQuizzApp.Domain.Entities;
using _Net6CleanArchitectureQuizzApp.Domain.Enums;

namespace _Net6CleanArchitectureQuizzApp.Application.QuestionDev.Queries.GetQuestionsByTestId;

public class GetQuestionDto : IMapFrom<Question>
{
    // ✅ AJOUTÉ : ID de la question (manquait dans la version originale)
    public int Id { get; set; }

    public string Content { get; set; } = null!;
    public QuestionType Type { get; set; }
    public string? AnswerDetails { get; set; }
    public int QuizTestId { get; set; }

    // ✅ CORRIGÉ : Utiliser QuestionChoice[] au lieu de object
    public QuestionChoice[] Choices { get; set; } = new QuestionChoice[0];

    // ✅ AJOUTÉ : IDs des réponses correctes
    public string? ListOfCorrectAnswerIds { get; set; }
}