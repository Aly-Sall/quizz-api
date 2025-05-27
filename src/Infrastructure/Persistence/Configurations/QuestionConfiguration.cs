using System.Reflection.Emit;
using System.Reflection.Metadata;
using _Net6CleanArchitectureQuizzApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Hosting;

namespace _Net6CleanArchitectureQuizzApp.Infrastructure.Persistence.Configurations;

public class QuestionConfiguration : IEntityTypeConfiguration<Question>
{
    public void Configure(EntityTypeBuilder<Question> builder)
    {
        builder
        .HasMany(e => e.QuizTests)
        .WithMany(e => e.Questions)
        .UsingEntity("TestQuestionJoinTable");

    }
}
