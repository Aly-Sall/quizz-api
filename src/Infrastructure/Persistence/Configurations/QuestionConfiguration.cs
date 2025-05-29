// src/Infrastructure/Persistence/Configurations/QuestionConfiguration.cs - NOUVEAU FICHIER
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using _Net6CleanArchitectureQuizzApp.Domain.Entities;

namespace _Net6CleanArchitectureQuizzApp.Infrastructure.Persistence.Configurations;

public class QuestionConfiguration : IEntityTypeConfiguration<Question>
{
    public void Configure(EntityTypeBuilder<Question> builder)
    {
        // Configuration de la table
        builder.ToTable("Questions");

        // Clé primaire
        builder.HasKey(q => q.Id);

        // Propriétés
        builder.Property(q => q.Content)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(q => q.AnswerDetails)
            .HasMaxLength(2000);

        builder.Property(q => q._Choices)
            .IsRequired()
            .HasColumnName("Choices")
            .HasColumnType("nvarchar(max)");

        builder.Property(q => q.ListOfCorrectAnswerIds)
            .IsRequired()
            .HasMaxLength(500)
            .HasDefaultValue("[]");

        // ✅ Configuration de la relation avec QuizTest
        builder.HasOne(q => q.QuizTest)
            .WithMany(t => t.Questions)
            .HasForeignKey(q => q.QuizTestId)
            .OnDelete(DeleteBehavior.Cascade); // Supprimer les questions si le test est supprimé

        // Ignorer la propriété Choices (elle est [NotMapped])
        builder.Ignore(q => q.Choices);

        // Index pour améliorer les performances
        builder.HasIndex(q => q.QuizTestId)
            .HasDatabaseName("IX_Questions_QuizTestId");
    }
}

// src/Infrastructure/Persistence/Configurations/QuizTestConfiguration.cs - NOUVEAU FICHIER
public class QuizTestConfiguration : IEntityTypeConfiguration<QuizTest>
{
    public void Configure(EntityTypeBuilder<QuizTest> builder)
    {
        // Configuration de la table
        builder.ToTable("QuizTests");

        // Clé primaire
        builder.HasKey(t => t.Id);

        // Propriétés
        builder.Property(t => t.Title)
            .HasMaxLength(200);

        builder.Property(t => t.IsActive)
            .HasDefaultValue(false);

        builder.Property(t => t.Duration)
            .HasDefaultValue(30); // 30 minutes par défaut

        // ✅ Configuration de la relation avec Questions
        builder.HasMany(t => t.Questions)
            .WithOne(q => q.QuizTest)
            .HasForeignKey(q => q.QuizTestId)
            .OnDelete(DeleteBehavior.Cascade);

        // Index pour améliorer les performances
        builder.HasIndex(t => t.IsActive)
            .HasDatabaseName("IX_QuizTests_IsActive");

        builder.HasIndex(t => t.Category)
            .HasDatabaseName("IX_QuizTests_Category");
    }
}