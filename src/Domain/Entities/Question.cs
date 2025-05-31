// src/Domain/Entities/Question.cs - VERSION CORRIGÉE COMPLÈTE
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using _Net6CleanArchitectureQuizzApp.Domain.Enums;
using _Net6CleanArchitectureQuizzApp.Domain.Interfaces;

namespace _Net6CleanArchitectureQuizzApp.Domain.Entities;

public class Question : IEntity
{
    public int Id { get; set; }
    public string Content { get; set; } = null!;
    public QuestionType Type { get; set; }
    public string? AnswerDetails { get; set; }

    // ✅ AJOUTÉ : Foreign Key pour la relation One-to-Many
    public int QuizTestId { get; set; }

    // ✅ CORRIGÉ : Navigation property pour le test parent
    public QuizTest QuizTest { get; set; } = null!;

    // Stockage JSON des choix
    public string _Choices { get; set; } = null!;

    [NotMapped]
    public QuestionChoice[]? Choices
    {
        get
        {
            if (string.IsNullOrEmpty(_Choices)) return new QuestionChoice[0];
            try
            {
                return JsonConvert.DeserializeObject<QuestionChoice[]>(_Choices) ?? new QuestionChoice[0];
            }
            catch
            {
                return new QuestionChoice[0];
            }
        }
        set
        {
            _Choices = JsonConvert.SerializeObject(value ?? new QuestionChoice[0]);
        }
    }

    // IDs des réponses correctes au format JSON
    public string ListOfCorrectAnswerIds { get; set; } = "[]";

    // ✅ SIMPLIFIÉ : Relation One-to-Many avec les réponses des candidats
    public ICollection<Reponse>? Reponses { get; set; } = new List<Reponse>();
   // public List<QuizTest> QuizTests { get; set; }
}

public class QuestionChoice
{
    public int Id { get; set; }
    public string Content { get; set; } = "";
}