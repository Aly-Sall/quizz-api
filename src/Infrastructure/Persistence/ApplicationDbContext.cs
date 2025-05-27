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
    public DbSet<Tentative> Tentatives =>  Set<Tentative>();
    public DbSet<TestAccessToken> TestAccessTokens => Set<TestAccessToken>();



    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(builder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {

        return await base.SaveChangesAsync(cancellationToken);
    }
}
