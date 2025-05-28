// src/Application/TestDev/Queries/GetAllTests/GetAllTestsQuery.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _Net6CleanArchitectureQuizzApp.Application.Common.Interfaces;
using _Net6CleanArchitectureQuizzApp.Application.Common.Mappings;
using _Net6CleanArchitectureQuizzApp.Application.Common.Models;
using _Net6CleanArchitectureQuizzApp.Domain.Entities;
using _Net6CleanArchitectureQuizzApp.Domain.Enums;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace _Net6CleanArchitectureQuizzApp.Application.TestDev.Queries.GetAllTests;

public record GetAllTestsQuery : IRequest<List<QuizTestDto>>
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 50;
    public bool ActiveOnly { get; set; } = false;
}

public class GetAllTestsHandler : IRequestHandler<GetAllTestsQuery, List<QuizTestDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetAllTestsHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<QuizTestDto>> Handle(GetAllTestsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Tests.AsQueryable();

        // Filtrer par statut actif si demandé
        if (request.ActiveOnly)
        {
            query = query.Where(t => t.IsActive);
        }

        // Appliquer la pagination
        var tests = await query
            .OrderByDescending(t => t.Id) // Les plus récents en premier
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(t => new QuizTestDto
            {
                Id = t.Id,
                Title = t.Title,
                Category = t.Category,
                Mode = t.Mode,
                TryAgain = t.TryAgain,
                ShowTimer = t.ShowTimer,
                Level = t.Level,
                IsActive = t.IsActive,
                Duration = t.Duration
            })
            .ToListAsync(cancellationToken);

        return tests;
    }
}

public class QuizTestDto : IMapFrom<QuizTest>
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
}