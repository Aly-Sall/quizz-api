// src/Domain/Entities/QuizTest.cs - VERSION CORRIGÉE
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace _Net6CleanArchitectureQuizzApp.Domain.Entities;

public class QuizTest : IEntity
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public Category Category { get; set; }
    public Mode Mode { get; set; }
    public bool TryAgain { get; set; }
    public bool ShowTimer { get; set; }
    public Level Level { get; set; }
    public bool IsActive { get; set; }
    public int Duration { get; set; }

    // ✅ CORRIGÉ : Relation One-to-Many avec Questions
    // Un test peut avoir plusieurs questions, chaque question appartient à un seul test
    public ICollection<Question>? Questions { get; set; } = new List<Question>();

    // Autres relations...
    //public ICollection<AnalyseIA>? AnalyseIAs { get; set; }
}