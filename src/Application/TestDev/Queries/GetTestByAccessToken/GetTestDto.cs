using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _Net6CleanArchitectureQuizzApp.Application.Common.Mappings;
using _Net6CleanArchitectureQuizzApp.Domain.Entities;
using _Net6CleanArchitectureQuizzApp.Domain.Enums;
using OpenAI.RealtimeConversation;

namespace _Net6CleanArchitectureQuizzApp.Application.QuestionDev.Queries.GetQuestionsByTestId;
public class GetTestDto : IMapFrom<QuizTest>
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public bool TryAgain { get; set; }
    public bool ShowTimer { get; set; }
    public bool IsActive { get; set; }
    public Mode Mode { get; set; }
    public int Duration { get; set; }   

    public ICollection<Question>? Questions { get; set; }
}
