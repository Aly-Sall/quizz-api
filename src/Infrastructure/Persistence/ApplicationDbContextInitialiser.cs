using _Net6CleanArchitectureQuizzApp.Domain.Entities;
using _Net6CleanArchitectureQuizzApp.Domain.Enums;
using _Net6CleanArchitectureQuizzApp.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace _Net6CleanArchitectureQuizzApp.Infrastructure.Persistence;

public class ApplicationDbContextInitialiser
{
    private readonly ILogger<ApplicationDbContextInitialiser> _logger;
    private readonly ApplicationDbContext _context;
    private readonly UserManager<User> _userManager;
    //private readonly RoleManager<IdentityRole> _roleManager;

    public ApplicationDbContextInitialiser(ILogger<ApplicationDbContextInitialiser> logger, ApplicationDbContext context, UserManager<User> userManager)
    {
        _logger = logger;
        _context = context;
        _userManager = userManager;
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
        // Default roles
        //var administratorRole = new IdentityRole("Administrator");

        //if (_roleManager.Roles.All(r => r.Name != administratorRole.Name))
        //{
        //    await _roleManager.CreateAsync(administratorRole);
        //}

        // Default users
        var administrator = new User { Prenom = "Rim", Nom = "Kachai", UserName = "administratorlocalhost", Email = "administrator@localhost" };

        if (_userManager.Users.All(u => u.UserName != administrator.UserName))
        {
            await _userManager.CreateAsync(administrator, "Administrator1!");
            //await _userManager.AddToRolesAsync(administrator, new[] { administratorRole.Name });
        }

        // Default data
        // Seed, if necessary
        if (!await _context.Tests.AnyAsync())
        {
            await _context.Tests.AddRangeAsync(
                new QuizTest
                {
                    Title = "General Knowledge Quiz",
                    Category = Category.General,
                    Mode = Mode.Training,
                    TryAgain = true,
                    ShowTimer = false,
                    Level = Level.Medium,
                    IsActive = true
                },
                new QuizTest
                {
                    Title = "AWS Certification Test",
                    Category = Category.Technical,
                    Mode = Mode.Recrutement,
                    TryAgain = false,
                    ShowTimer = true,
                    Level = Level.Hard,
                    IsActive = false
                },
                new QuizTest
                {
                    Title = "Safety Procedures Training",
                    Category = Category.General,
                    Mode = Mode.Training,
                    TryAgain = true,
                    ShowTimer = true,
                    Level = Level.Medium,
                    IsActive = true
                }
            );

            await _context.SaveChangesAsync();
        }
        if (!await _context.Questions.AnyAsync())
        {
            var questions = new List<Question>
    {
        new Question
        {
            Content = "What does AWS CloudWatch do?",
            Type = QuestionType.SingleChoice,
            AnswerDetails = "AWS CloudWatch monitors AWS resources and applications in real time.",
            Choices = new[]
            {
                new QuestionChoice { Id = 1, Content = "Monitors resources" },
                new QuestionChoice { Id = 2, Content = "Transfers data" },
                new QuestionChoice { Id = 3, Content = "Creates backups" },
                new QuestionChoice { Id = 4, Content = "Deletes databases" }
            },
            ListOfCorrectAnswerIds = "[1]"
        },
        new Question
        {
            Content = "Select all storage services in AWS.",
            Type = QuestionType.MultiChoice,
            AnswerDetails = "S3 and EBS are storage services.",
            Choices = new[]
            {
                new QuestionChoice { Id = 1, Content = "Amazon S3" },
                new QuestionChoice { Id = 2, Content = "Amazon EC2" },
                new QuestionChoice { Id = 3, Content = "Amazon EBS" },
                new QuestionChoice { Id = 4, Content = "AWS Lambda" }
            },
            ListOfCorrectAnswerIds = "[1,3]"
        },
        new Question
        {
            Content = "What is the default region when launching an EC2 instance?",
            Type = QuestionType.SingleChoice,
            AnswerDetails = "The default region is the one closest to you unless changed manually.",
            Choices = new[]
            {
                new QuestionChoice { Id = 1, Content = "us-east-1" },
                new QuestionChoice { Id = 2, Content = "us-west-2" },
                new QuestionChoice { Id = 3, Content = "eu-west-1" },
                new QuestionChoice { Id = 4, Content = "ap-southeast-1" }
            },
            ListOfCorrectAnswerIds = "[1]"
        }
    };

            await _context.Questions.AddRangeAsync(questions);
            await _context.SaveChangesAsync();
            

        }
        if (!await _context.Tentatives.AnyAsync())
        {
            _context.Tentatives.Add(new Tentative
            {
                ScoreObtenu = 85.5f,
                PassingDate = DateTime.UtcNow,
                TestId = 1
            });
            await _context.SaveChangesAsync();
        }
        var test1 = await _context.Tests
        .Include(t => t.Questions)
        .FirstOrDefaultAsync(t => t.Id == 1);
        if (test1 != null)
        {
            foreach (var question in test1.Questions)
            {
                // Parse the stored JSON choices
                var choices = question.Choices;

                if (choices != null && choices.Length > 0)
                {
                    var selectedChoice = choices[0];

                    _context.Responses.Add(new Reponse
                    {
                        ChoiceId = selectedChoice.Id,
                        QuestionId = question.Id,
                        QuizTestId = test1.Id
                    });
                }
            }

            await _context.SaveChangesAsync();
        }
        if (!await _context.TestAccessTokens.AnyAsync())
        {
            TestAccessToken tokn = new TestAccessToken
            {
                CandidateEmail = "candidate@example.com",
                ExpirationTime = DateTime.UtcNow.AddHours(2),
                IsUsed = false,
                TestId = 1,
                TentativeId = null
            };
            _context.TestAccessTokens.Add(tokn);
            await _context.SaveChangesAsync();
        }
        var test = await _context.Tests.FirstOrDefaultAsync();
        if (test != null)
        {
            // Get all existing questions
            var allQuestions = await _context.Questions.ToListAsync();
            foreach (var question in allQuestions)
            {
                //there is no initialization of collection !! important what about others !! 

                question.QuizTests.Add(test);
            }

            await _context.SaveChangesAsync();
        }

    }
}
