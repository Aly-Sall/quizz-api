using _Net6CleanArchitectureQuizzApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace _Net6CleanArchitectureQuizzApp.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<QuizTest> Tests { get; }

    DbSet<Question> Questions { get; }
    DbSet<Reponse> Responses { get; }
    DbSet<Surveillance> Surveillances { get; }
    DbSet<Tentative> Tentatives { get; }
    DbSet<TestAccessToken> TestAccessTokens { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    Task<int> SaveChangesAsync();
}
