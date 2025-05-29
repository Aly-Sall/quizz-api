using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace _Net6CleanArchitectureQuizzApp.Domain.Entities;

public class Question : IEntity
{
    public int Id { get; set; }

    public string Content { get; set; } = null!;

    public QuestionType Type { get; set; }

    public string? AnswerDetails { get; set; }

    // Stockage JSON des choix
    public string _Choices { get; set; } = "[]"; // Initialiser avec un tableau JSON vide

    [NotMapped]
    public QuestionChoice[]? Choices
    {
        get
        {
            try
            {
                return string.IsNullOrEmpty(_Choices) || _Choices == "[]"
                    ? new QuestionChoice[0]
                    : JsonConvert.DeserializeObject<QuestionChoice[]>(_Choices);
            }
            catch
            {
                return new QuestionChoice[0];
            }
        }
        set
        {
            try
            {
                _Choices = value == null || value.Length == 0
                    ? "[]"
                    : JsonConvert.SerializeObject(value);
            }
            catch
            {
                _Choices = "[]";
            }
        }
    }

    // Relation many-to-many avec QuizTest
    public ICollection<QuizTest> QuizTests { get; set; } = new List<QuizTest>();

    public string ListOfCorrectAnswerIds { get; set; } = "[]";

    // Response is the candidate answer 
    public ICollection<Reponse>? Reponses { get; set; } = new List<Reponse>();
}

public class QuestionChoice
{
    public int Id { get; set; }
    public string Content { get; set; } = null!;
}