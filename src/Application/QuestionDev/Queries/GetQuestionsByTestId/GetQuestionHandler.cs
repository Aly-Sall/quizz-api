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
        var questions = await _context.Tests
            .Where(t => t.Id == request.Id)
            .SelectMany(t => t.Questions) 
            .ProjectTo<GetQuestionDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        return questions;
    }
}