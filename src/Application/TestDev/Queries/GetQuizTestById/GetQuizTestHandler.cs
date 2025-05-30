// src/Application/TestDev/Queries/GetQuizTestById/GetQuizTestHandler.cs - VERSION CORRIGÉE
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _Net6CleanArchitectureQuizzApp.Application.Common.Interfaces;
using _Net6CleanArchitectureQuizzApp.Application.Common.Models;
using _Net6CleanArchitectureQuizzApp.Domain.Entities;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static System.Collections.Specialized.BitVector32;

namespace _Net6CleanArchitectureQuizzApp.Application.TestDev.Queries.GetQuizTestById;

public record GetQuizTestQuery : IRequest<Result<QuizTest>>
{
    //Query that will be sent , parameters
    public int Id { get; set; }
}

public class GetQuizTestHandler : IRequestHandler<GetQuizTestQuery, Result<QuizTest>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetQuizTestHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Result<QuizTest>> Handle(GetQuizTestQuery request, CancellationToken cancellationToken)
    {
        var quiz = await _context.Tests
                .AsNoTracking()
            .Where(q => q.Id == request.Id) // ✅ CORRIGÉ : Utiliser request.Id au lieu de 1
            .Include(q => q.Questions)
            .Select(q => new QuizTest
            {
                Id = q.Id,
                Title = q.Title,
                Category = q.Category,
                Mode = q.Mode,
                TryAgain = q.TryAgain,
                ShowTimer = q.ShowTimer,
                Level = q.Level,
                IsActive = q.IsActive,
                Questions = q.Questions.Select(x => new Question
                {
                    Id = x.Id,
                    Content = x.Content,
                    Type = x.Type,
                    AnswerDetails = x.AnswerDetails,
                    _Choices = x._Choices,
                    ListOfCorrectAnswerIds = x.ListOfCorrectAnswerIds,
                    // ✅ CORRIGÉ : Assigner les bonnes propriétés selon le nouveau modèle
                    QuizTestId = x.QuizTestId,
                    // Note: On n'assigne pas QuizTest pour éviter la référence circulaire
                }).ToList()
            })
            .FirstOrDefaultAsync();

        if (quiz == null)
        {
            return Result<QuizTest>.Failure("Test not found");
        }

        return Result<QuizTest>.Success(quiz);
    }
}