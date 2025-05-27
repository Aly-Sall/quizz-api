using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _Net6CleanArchitectureQuizzApp.Application.Common.Exceptions;
using _Net6CleanArchitectureQuizzApp.Application.Common.Interfaces;
using _Net6CleanArchitectureQuizzApp.Application.Common.Mappings;
using _Net6CleanArchitectureQuizzApp.Application.Common.Models;
using _Net6CleanArchitectureQuizzApp.Application.TestDev.Queries.GetQuizTestById;
using _Net6CleanArchitectureQuizzApp.Domain.Entities;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace _Net6CleanArchitectureQuizzApp.Application.QuestionDev.Queries.GetQuestionsByTestId;
public record GetQuizTestByTokenQuery : IRequest<GetTestDto>
{
    public string token { get; set; }

}
public class GetQuizTestHandler : IRequestHandler<GetQuizTestByTokenQuery, GetTestDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetQuizTestHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<GetTestDto> Handle(GetQuizTestByTokenQuery request, CancellationToken cancellationToken)
    {
        var RegisteredToken = await _context.TestAccessTokens
             .Where(x => x.Token == request.token)
             .FirstOrDefaultAsync(); 
        if(RegisteredToken == null)
        {
            throw new NotFoundException("QuizTest", request.token);
        }
        var test = await _context.Tests
             .Where(x => x.Id == RegisteredToken.Id)
             .FirstOrDefaultAsync();
        return _mapper.Map<GetTestDto>(test);
    }
}