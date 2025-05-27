using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _Net6CleanArchitectureQuizzApp.Application.Common.Exceptions;
using _Net6CleanArchitectureQuizzApp.Application.Common.Interfaces;
using _Net6CleanArchitectureQuizzApp.Application.Common.Mappings;
using _Net6CleanArchitectureQuizzApp.Application.QuestionDev.Queries.GetQuestionsByTestId;
using _Net6CleanArchitectureQuizzApp.Domain.Entities;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace _Net6CleanArchitectureQuizzApp.Application.TestAccessTokenDev.Queries.GetTokenDetailsByToken;
public class GetTokenDto : IMapFrom<TestAccessToken>
{
    public DateTime ExpirationTime { get; set; }
    public bool IsUsed { get; set; } = false;
}
public class GetTokenDetailsQuery : IRequest<GetTokenDto>
{
    public string Token { get; set; }
}
    public class GetTokenDetailsHandler : IRequestHandler<GetTokenDetailsQuery,GetTokenDto>
    {
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetTokenDetailsHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<GetTokenDto> Handle(GetTokenDetailsQuery request, CancellationToken cancellationToken)
    {
        var RegisteredToken = await _context.TestAccessTokens
             .Where(x => x.Token == request.Token)
             .FirstOrDefaultAsync();

        if (RegisteredToken == null)
        {
            throw new NotFoundException("TokenAccess", request.Token);
        }
        
        return _mapper.Map<GetTokenDto>(RegisteredToken);
    }
    }

