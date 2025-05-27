using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _Net6CleanArchitectureQuizzApp.Application.Common.Mappings;
using _Net6CleanArchitectureQuizzApp.Domain.Entities;
using _Net6CleanArchitectureQuizzApp.Domain.Enums;
using Newtonsoft.Json;

namespace _Net6CleanArchitectureQuizzApp.Application.TestDev.Queries.GetQuizTestById;
public class QuizTestDto : IMapFrom<QuizTest>
{
    public string? Title { get; set; }
    public Category Category { get; set; }
    public Mode Mode { get; set; }
    public bool TryAgain { get; set; }
    public bool ShowTimer { get; set; }
    public Level Level { get; set; }
    public bool IsActive { get; set; }

    public ICollection<QuestionDto> QuestionDtos { get; set; }
}
public class QuestionDto : IMapFrom<Question>
{
    public int Id { get; set; }

    public string Content { get; set; } = null!;

    public QuestionType Type { get; set; }

    public string? AnswerDetails { get; set; }

    public QuestionChoice[] Choices { get; set; } 

    public string ListOfCorrectAnswerIds { get; set; } 
}


