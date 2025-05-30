// src/Infrastructure/Persistence/ApplicationDbContext.cs - VERSION CORRIGÉE
using System.Reflection;
using _Net6CleanArchitectureQuizzApp.Application.Common.Interfaces;
using _Net6CleanArchitectureQuizzApp.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace _Net6CleanArchitectureQuizzApp.Infrastructure.Persistence;

// ✅ CORRIGÉ : Utiliser User au lieu de IdentityUser<int>
public class ApplicationDbContext : IdentityDbContext<User, IdentityRole<int>, int>, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    // ✅ DbSets pour les entités
    public DbSet<QuizTest> Tests => Set<QuizTest>();
    public DbSet<Question> Questions => Set<Question>();
    public DbSet<Reponse> Responses => Set<Reponse>();
    public DbSet<Surveillance> Surveillances => Set<Surveillance>();
    public DbSet<Tentative> Tentatives => Set<Tentative>();
    public DbSet<TestAccessToken> TestAccessTokens => Set<TestAccessToken>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        // ✅ Configuration des entités avec la NOUVELLE relation One-to-Many

        // Configuration QuizTest
        builder.Entity<QuizTest>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Title)
                .HasMaxLength(200);

            entity.Property(e => e.IsActive)
                .HasDefaultValue(false);

            entity.Property(e => e.Duration)
                .HasDefaultValue(30);

            // ✅ NOUVELLE relation One-to-Many avec Questions
            entity.HasMany(t => t.Questions)
                .WithOne(q => q.QuizTest)
                .HasForeignKey(q => q.QuizTestId)
                .OnDelete(DeleteBehavior.Cascade);

            // Index pour les performances
            entity.HasIndex(t => t.IsActive)
                .HasDatabaseName("IX_QuizTests_IsActive");
        });

        // Configuration Question
        builder.Entity<Question>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Content)
                .IsRequired()
                .HasMaxLength(1000);

            entity.Property(e => e.AnswerDetails)
                .HasMaxLength(2000);

            entity.Property(e => e._Choices)
                .IsRequired()
                .HasColumnName("Choices")
                .HasColumnType("nvarchar(max)");

            entity.Property(e => e.ListOfCorrectAnswerIds)
                .IsRequired()
                .HasMaxLength(500)
                .HasDefaultValue("[]");

            // ✅ NOUVELLE relation avec QuizTest (One-to-Many)
            entity.HasOne(q => q.QuizTest)
                .WithMany(t => t.Questions)
                .HasForeignKey(q => q.QuizTestId)
                .OnDelete(DeleteBehavior.Cascade);

            // Ignorer la propriété calculée Choices
            entity.Ignore(q => q.Choices);

            // Index pour les performances
            entity.HasIndex(q => q.QuizTestId)
                .HasDatabaseName("IX_Questions_QuizTestId");
        });

        // Configuration Reponse (réponses des candidats)
        builder.Entity<Reponse>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.HasOne(r => r.Question)
                .WithMany(q => q.Reponses)
                .HasForeignKey(r => r.QuestionId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(r => r.QuizTest)
                .WithMany()
                .HasForeignKey(r => r.QuizTestId)
                .OnDelete(DeleteBehavior.NoAction); // Éviter les cycles de suppression
        });

        // Configuration Tentative
        builder.Entity<Tentative>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.HasOne(t => t.Test)
                .WithMany()
                .HasForeignKey(t => t.TestId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(t => t.Surveillances)
                .WithOne(s => s.Tentative)
                .HasForeignKey(s => s.TentativeId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configuration Surveillance
        builder.Entity<Surveillance>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.TimeStamp)
                .HasDefaultValueSql("GETUTCDATE()");

            entity.HasOne(s => s.Tentative)
                .WithMany(t => t.Surveillances)
                .HasForeignKey(s => s.TentativeId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configuration TestAccessToken
        builder.Entity<TestAccessToken>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Token)
                .IsRequired()
                .HasMaxLength(255);

            entity.Property(e => e.CandidateEmail)
                .IsRequired()
                .HasMaxLength(255);

            entity.HasIndex(e => e.Token)
                .IsUnique()
                .HasDatabaseName("IX_TestAccessTokens_Token");
        });

        // ✅ Configuration Identity pour User personnalisé
        builder.Entity<User>(entity =>
        {
            entity.Property(e => e.Nom)
                .HasMaxLength(100);

            entity.Property(e => e.Prenom)
                .HasMaxLength(100);

            // Email est déjà configuré par Identity, mais on peut ajuster
            entity.Property(e => e.Email)
                .HasMaxLength(256);
        });

        // Appeler la configuration de base (Identity)
        base.OnModelCreating(builder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await base.SaveChangesAsync(cancellationToken);
    }

    public Task<int> SaveChangesAsync()
    {
        throw new NotImplementedException();
    }
}