// src/Application/Account/Models/TestDto.cs
namespace _Net6CleanArchitectureQuizzApp.Application.Account.Models;

/// <summary>
/// DTO pour représenter un test disponible
/// </summary>
public class TestDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Duration { get; set; } // en minutes
    public bool IsActive { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int QuestionCount { get; set; }
}