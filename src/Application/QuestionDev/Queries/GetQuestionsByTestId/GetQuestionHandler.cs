using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _Net6CleanArchitectureQuizzApp.Application.Common.Interfaces;
using _Net6CleanArchitectureQuizzApp.Application.Common.Mappings;
using _Net6CleanArchitectureQuizzApp.Application.Common.Models;
using _Net6CleanArchitectureQuizzApp.Application.TestDev.Queries.GetQuizTestById;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace _Net6CleanArchitectureQuizzApp.Application.QuestionDev.Queries.GetQuestionsByTestId;

public record GetQuestionQuery : IRequest<List<GetQuestionDto>>
{
    //Query that will be sent , parameters
    public int Id { get; set; }
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
        // DEBUG : Log the request
        Console.WriteLine($"GetQuestionHandler: Looking for questions for test ID {request.Id}");

        // Méthode 1 : Par association many-to-many
        var questionsFromAssociation = await _context.Tests
            .Where(t => t.Id == request.Id)
            .SelectMany(t => t.Questions)
            .Select(q => new GetQuestionDto
            {
                Id = q.Id,
                Content = q.Content,
                Type = q.Type,
                AnswerDetails = q.AnswerDetails,
                QuizTestId = request.Id,
                Choices = q.Choices,
                ListOfCorrectAnswerIds = q.ListOfCorrectAnswerIds
            })
            .ToListAsync(cancellationToken);

        Console.WriteLine($"Found {questionsFromAssociation.Count} questions from association");

        // Si pas de résultats, essayer toutes les questions (fallback pour debug)
        if (questionsFromAssociation.Count == 0)
        {
            Console.WriteLine("No questions found from association, trying all questions...");
            
            var allQuestions = await _context.Questions
                .Select(q => new GetQuestionDto
                {
                    Id = q.Id,
                    Content = q.Content,
                    Type = q.Type,
                    AnswerDetails = q.AnswerDetails,
                    QuizTestId = request.Id,
                    Choices = q.Choices,
                    ListOfCorrectAnswerIds = q.ListOfCorrectAnswerIds
                })
                .ToListAsync(cancellationToken);

            Console.WriteLine($"Found {allQuestions.Count} total questions in database");
            
            // Log des détails pour debug
            foreach (var q in allQuestions)
            {
                Console.WriteLine($"Question {q.Id}: '{q.Content}' - CorrectAnswers: '{q.ListOfCorrectAnswerIds}'");
            }

            return allQuestions;
        }

        // Log des détails pour debug
        foreach (var q in questionsFromAssociation)
        {
            Console.WriteLine($"Question {q.Id}: '{q.Content}' - CorrectAnswers: '{q.ListOfCorrectAnswerIds}'");
        }

        return questionsFromAssociation;
    }
}