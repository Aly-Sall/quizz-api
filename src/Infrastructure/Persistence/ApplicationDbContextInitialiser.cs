// src/Infrastructure/Persistence/ApplicationDbContextInitialiser.cs - VERSION CORRIGÉE
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using _Net6CleanArchitectureQuizzApp.Domain.Entities;
using _Net6CleanArchitectureQuizzApp.Domain.Enums;
using _Net6CleanArchitectureQuizzApp.Domain.Constants;

namespace _Net6CleanArchitectureQuizzApp.Infrastructure.Persistence;

public class ApplicationDbContextInitialiser
{
    private readonly ILogger<ApplicationDbContextInitialiser> _logger;
    private readonly ApplicationDbContext _context;
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole<int>> _roleManager;

    public ApplicationDbContextInitialiser(
        ILogger<ApplicationDbContextInitialiser> logger,
        ApplicationDbContext context,
        UserManager<User> userManager,
        RoleManager<IdentityRole<int>> roleManager)
    {
        _logger = logger;
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task InitialiseAsync()
    {
        try
        {
            if (_context.Database.IsSqlServer())
            {
                await _context.Database.MigrateAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while initialising the database.");
            throw;
        }
    }

    public async Task SeedAsync()
    {
        try
        {
            await TrySeedAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the database.");
            throw;
        }
    }

    public async Task TrySeedAsync()
    {
        // Seed Default Roles
        var administratorRole = new IdentityRole<int>(Roles.Administrator);

        if (_roleManager.Roles.All(r => r.Name != administratorRole.Name))
        {
            await _roleManager.CreateAsync(administratorRole);
        }

        // Seed Default User
        var administrator = new User
        {
            UserName = "administrator@localhost",
            Email = "administrator@localhost",
            Nom = "Admin",
            Prenom = "System"
        };

        if (_userManager.Users.All(u => u.UserName != administrator.UserName))
        {
            await _userManager.CreateAsync(administrator, "Administrator1!");
            if (!string.IsNullOrWhiteSpace(administratorRole.Name))
            {
                await _userManager.AddToRolesAsync(administrator, new[] { administratorRole.Name });
            }
        }

        // ✅ CORRIGÉ : Seed Tests avec la nouvelle relation One-to-Many
        if (!_context.Tests.Any())
        {
            var sampleTests = new List<QuizTest>
            {
                new QuizTest
                {
                    Title = "Angular Fundamentals",
                    Category = Category.Technical,
                    Mode = Mode.Training,
                    TryAgain = true,
                    ShowTimer = true,
                    Level = Level.Medium,
                    IsActive = true,
                    Duration = 30
                },
                new QuizTest
                {
                    Title = "General Knowledge Quiz",
                    Category = Category.General,
                    Mode = Mode.Training,
                    TryAgain = true,
                    ShowTimer = false,
                    Level = Level.Easy,
                    IsActive = true,
                    Duration = 20
                },
                new QuizTest
                {
                    Title = "Advanced Programming",
                    Category = Category.Technical,
                    Mode = Mode.Recrutement,
                    TryAgain = false,
                    ShowTimer = true,
                    Level = Level.Hard,
                    IsActive = true,
                    Duration = 45
                }
            };

            _context.Tests.AddRange(sampleTests);
            await _context.SaveChangesAsync();

            // ✅ CORRIGÉ : Seed Questions avec QuizTestId au lieu de QuizTests
            var questions = new List<Question>();

            // Questions pour le test Angular Fundamentals (ID = 1)
            var angularTest = sampleTests[0];
            questions.AddRange(new[]
            {
                new Question
                {
                    Content = "What is Angular?",
                    Type = QuestionType.SingleChoice,
                    AnswerDetails = "Angular is a TypeScript-based open-source web application framework",
                    QuizTestId = angularTest.Id, // ✅ CORRIGÉ : Utiliser QuizTestId
                    ListOfCorrectAnswerIds = "[1]",
                    Choices = new[]
                    {
                        new QuestionChoice { Id = 1, Content = "A TypeScript-based web framework" },
                        new QuestionChoice { Id = 2, Content = "A JavaScript library like jQuery" },
                        new QuestionChoice { Id = 3, Content = "A CSS framework" },
                        new QuestionChoice { Id = 4, Content = "A database management system" }
                    }
                },
                new Question
                {
                    Content = "Which of the following are Angular core concepts?",
                    Type = QuestionType.MultiChoice,
                    AnswerDetails = "Components, Services, and Directives are core Angular concepts",
                    QuizTestId = angularTest.Id, // ✅ CORRIGÉ : Utiliser QuizTestId
                    ListOfCorrectAnswerIds = "[1,2,3]",
                    Choices = new[]
                    {
                        new QuestionChoice { Id = 1, Content = "Components" },
                        new QuestionChoice { Id = 2, Content = "Services" },
                        new QuestionChoice { Id = 3, Content = "Directives" },
                        new QuestionChoice { Id = 4, Content = "Tables" }
                    }
                }
            });

            // Questions pour le test General Knowledge (ID = 2)
            var generalTest = sampleTests[1];
            questions.AddRange(new[]
            {
                new Question
                {
                    Content = "What is the capital of France?",
                    Type = QuestionType.SingleChoice,
                    AnswerDetails = "Paris is the capital and most populous city of France",
                    QuizTestId = generalTest.Id, // ✅ CORRIGÉ : Utiliser QuizTestId
                    ListOfCorrectAnswerIds = "[1]",
                    Choices = new[]
                    {
                        new QuestionChoice { Id = 1, Content = "Paris" },
                        new QuestionChoice { Id = 2, Content = "London" },
                        new QuestionChoice { Id = 3, Content = "Berlin" },
                        new QuestionChoice { Id = 4, Content = "Madrid" }
                    }
                },
                new Question
                {
                    Content = "Which of these are programming languages?",
                    Type = QuestionType.MultiChoice,
                    AnswerDetails = "C#, Java, and Python are all programming languages",
                    QuizTestId = generalTest.Id, // ✅ CORRIGÉ : Utiliser QuizTestId
                    ListOfCorrectAnswerIds = "[1,2,3]",
                    Choices = new[]
                    {
                        new QuestionChoice { Id = 1, Content = "C#" },
                        new QuestionChoice { Id = 2, Content = "Java" },
                        new QuestionChoice { Id = 3, Content = "Python" },
                        new QuestionChoice { Id = 4, Content = "HTML" }
                    }
                }
            });

            // Questions pour le test Advanced Programming (ID = 3)
            var advancedTest = sampleTests[2];
            questions.AddRange(new[]
            {
                new Question
                {
                    Content = "What is the time complexity of binary search?",
                    Type = QuestionType.SingleChoice,
                    AnswerDetails = "Binary search has O(log n) time complexity",
                    QuizTestId = advancedTest.Id, // ✅ CORRIGÉ : Utiliser QuizTestId
                    ListOfCorrectAnswerIds = "[2]",
                    Choices = new[]
                    {
                        new QuestionChoice { Id = 1, Content = "O(n)" },
                        new QuestionChoice { Id = 2, Content = "O(log n)" },
                        new QuestionChoice { Id = 3, Content = "O(n²)" },
                        new QuestionChoice { Id = 4, Content = "O(1)" }
                    }
                }
            });

            _context.Questions.AddRange(questions);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Database seeded successfully with {TestCount} tests and {QuestionCount} questions",
                sampleTests.Count, questions.Count);
        }
    }
}