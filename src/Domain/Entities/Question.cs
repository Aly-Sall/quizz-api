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
    public string _Choices { get; set; } = null!; 
    [NotMapped]
    public QuestionChoice[]? Choices
    {
        get { return  JsonConvert.DeserializeObject<QuestionChoice[]>(_Choices); }
        set { _Choices = JsonConvert.SerializeObject(value); }
    }
    public ICollection<QuizTest> QuizTests { get; set; } = new List<QuizTest>();
    public string ListOfCorrectAnswerIds { get; set; }

    //Response is the candidate answer 
    public ICollection<Reponse>? Reponses { get; set; } = new List<Reponse>();


}
public class QuestionChoice
{
    public int Id { get; set; }
    public string Content { get; set; } = null!;
}

