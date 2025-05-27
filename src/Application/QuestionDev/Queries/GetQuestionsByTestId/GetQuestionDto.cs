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
    public string Content { get; set; } = null!;
    public QuestionType Type { get; set; }
    public string? AnswerDetails { get; set; }
    public int QuizTestId { get; set; }
    public QuestionChoice[] Choices { get; set; }   
}
