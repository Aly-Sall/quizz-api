// src/Application/QuestionDev/Queries/GetQuestionsByTestId/GetQuestionHandler.cs - VERSION CORRIGÉE
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _Net6CleanArchitectureQuizzApp.Application.Common.Interfaces;
using _Net6CleanArchitectureQuizzApp.Application.Common.Mappings;
using _Net6CleanArchitectureQuizzApp.Application.Common.Models;
using _Net6CleanArchitectureQuizzApp.Application.TestDev.Queries.GetQuizTestById;
using _Net6CleanArchitectureQuizzApp.Domain.Entities;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace _Net6CleanArchitectureQuizzApp.Application.QuestionDev.Queries.GetQuestionsByTestId;

public record GetQuestionQuery : IRequest<List<GetQuestionDto>>
{
    public int Id { get; set; } // ID du test
}

public class GetQuestionHandler : IRequestHandler<GetQuestionQuery, List<GetQuestionDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetQuestionHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<GetQuestionDto>> Handle(GetQuestionQuery request, CancellationToken cancellationToken)
    {
        // ✅ CORRIGÉ : Utiliser la nouvelle relation directe via QuizTestId
        var questions = await _context.Questions
            .Where(q => q.QuizTestId == request.Id) // Relation directe via Foreign Key
            .Select(q => new GetQuestionDto
            {
                Id = q.Id, // ✅ AJOUTÉ : ID de la question
                Content = q.Content,
                Type = q.Type,
                AnswerDetails = q.AnswerDetails,
                QuizTestId = q.QuizTestId,
                Choices = q.Choices ?? new QuestionChoice[0], // ✅ Gérer le cas null
                ListOfCorrectAnswerIds = q.ListOfCorrectAnswerIds
            })
            .OrderBy(q => q.Id) // Ordonner par ID pour un affichage cohérent
            .ToListAsync(cancellationToken);

        return questions;
    }
}