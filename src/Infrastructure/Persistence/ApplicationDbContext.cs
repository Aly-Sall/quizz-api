using System.Reflection;
using _Net6CleanArchitectureQuizzApp.Application.Common.Interfaces;
using _Net6CleanArchitectureQuizzApp.Domain.Entities;
using _Net6CleanArchitectureQuizzApp.Infrastructure.Identity;
using Duende.IdentityServer.EntityFramework.Options;
using MediatR;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace _Net6CleanArchitectureQuizzApp.Infrastructure.Persistence;

public class ApplicationDbContext : IdentityUserContext<User, int>, IApplicationDbContext
{
    private readonly IMediator _mediator;

    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        IMediator mediator)
        : base(options)
    {
        _mediator = mediator;
    }

    public DbSet<QuizTest> Tests => Set<QuizTest>();
    public DbSet<Question> Questions => Set<Question>();
    public DbSet<Reponse> Responses => Set<Reponse>();
    public DbSet<Surveillance> Surveillances => Set<Surveillance>();
    public DbSet<Tentative> Tentatives => Set<Tentative>();
    public DbSet<TestAccessToken> TestAccessTokens => Set<TestAccessToken>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        // Configuration des entités personnalisées
        ConfigureCustomEntities(builder);

        // Appliquer les configurations depuis l'assembly
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        // Configuration Identity
        base.OnModelCreating(builder);
    }

    private void ConfigureCustomEntities(ModelBuilder builder)
    {
        // Configuration de QuizTest
        builder.Entity<QuizTest>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title)
                .IsRequired()
                .HasMaxLength(200);
            entity.Property(e => e.Category)
                .IsRequired();
            entity.Property(e => e.Mode)
                .IsRequired();
            entity.Property(e => e.Level)
                .IsRequired();
            entity.Property(e => e.TryAgain)
                .IsRequired();
            entity.Property(e => e.ShowTimer)
                .IsRequired();
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true);
            entity.Property(e => e.Duration)
                .HasDefaultValue(30); // 30 minutes par défaut
        });

        // Configuration de Question
        builder.Entity<Question>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Content)
                .IsRequired()
                .HasMaxLength(1000);
            entity.Property(e => e.Type)
                .IsRequired();
            entity.Property(e => e.AnswerDetails)
                .HasMaxLength(2000);
            entity.Property(e => e._Choices)
                .IsRequired()
                .HasColumnName("Choices");
            entity.Property(e => e.ListOfCorrectAnswerIds)
                .IsRequired();

            // IMPORTANT: Configuration de la relation Many-to-Many avec QuizTest
            entity.HasMany(q => q.QuizTests)
                .WithMany(t => t.Questions)
                .UsingEntity("QuestionQuizTest"); // Table de jonction
        });

        // Configuration de Reponse (candidate answer)
        builder.Entity<Reponse>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ChoiceId)
                .IsRequired();
            entity.Property(e => e.QuestionId)
                .IsRequired();
            entity.Property(e => e.QuizTestId)
                .IsRequired();

            // Relations avec Question et QuizTest
            entity.HasOne(r => r.Question)
                .WithMany(q => q.Reponses)
                .HasForeignKey(r => r.QuestionId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(r => r.QuizTest)
                .WithMany()
                .HasForeignKey(r => r.QuizTestId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configuration de Tentative
        builder.Entity<Tentative>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ScoreObtenu)
                .IsRequired()
                .HasDefaultValue(0);
            entity.Property(e => e.PassingDate)
                .IsRequired();
            entity.Property(e => e.TestId)
                .IsRequired();

            // Relation avec QuizTest
            entity.HasOne(t => t.Test)
                .WithMany()
                .HasForeignKey(t => t.TestId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configuration de Surveillance
        builder.Entity<Surveillance>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ComportementSuspect)
                .HasDefaultValue(false);
            entity.Property(e => e.TimeStamp)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");
            entity.Property(e => e.CaptureEcran)
                .IsRequired();
            entity.Property(e => e.TentativeId)
                .IsRequired();

            // Relation avec Tentative
            entity.HasOne(s => s.Tentative)
                .WithMany(t => t.Surveillances)
                .HasForeignKey(s => s.TentativeId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configuration de TestAccessToken
        builder.Entity<TestAccessToken>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Token)
                .IsRequired()
                .HasMaxLength(50);
            entity.Property(e => e.CandidateEmail)
                .IsRequired()
                .HasMaxLength(255);
            entity.Property(e => e.ExpirationTime)
                .IsRequired();
            entity.Property(e => e.IsUsed)
                .HasDefaultValue(false);
            entity.Property(e => e.TestId)
                .IsRequired();

            // Index unique sur le token pour les performances
            entity.HasIndex(e => e.Token)
                .IsUnique();

            // Index sur l'email pour les recherches
            entity.HasIndex(e => e.CandidateEmail);
        });

        // Configuration de User (hérite d'IdentityUser)
        builder.Entity<User>(entity =>
        {
            entity.Property(e => e.Nom)
                .HasMaxLength(100);
            entity.Property(e => e.Prenom)
                .HasMaxLength(100);
            entity.Property(e => e.Email)
                .HasMaxLength(255);
        });
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Configuration additionnelle si nécessaire
        if (!optionsBuilder.IsConfigured)
        {
            // Configuration par défaut si aucune n'est fournie
            // Généralement, ceci est configuré dans Program.cs ou Startup.cs
        }
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Ici vous pouvez ajouter de la logique avant la sauvegarde
        // Par exemple : audit trail, timestamps automatiques, etc.

        return await base.SaveChangesAsync(cancellationToken);
    }
}