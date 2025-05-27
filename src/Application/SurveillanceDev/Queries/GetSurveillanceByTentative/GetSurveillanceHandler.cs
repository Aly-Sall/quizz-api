using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _Net6CleanArchitectureQuizzApp.Application.Common.Interfaces;
using _Net6CleanArchitectureQuizzApp.Application.Common.Mappings;
using _Net6CleanArchitectureQuizzApp.Application.QuestionDev.Queries.GetQuestionsByTestId;
using AutoMapper;
using MediatR;

namespace _Net6CleanArchitectureQuizzApp.Application.SurveillanceDev.Queries.GetSurveillanceByTentative;
public class GetSurveillanceQuery : IRequest<List<GetSurveillanceDto>>
{
    //Query that will be sent , parameters
    //Get surveillances informations depending on an tentative
    public int Id { get; set; }

}
public class GetSurveillanceHandler : IRequestHandler<GetSurveillanceQuery, List<GetSurveillanceDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetSurveillanceHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<GetSurveillanceDto>> Handle(GetSurveillanceQuery request, CancellationToken cancellationToken)
    {
        return await _context.Surveillances
             .Where(x => x.TentativeId == request.Id)
             .ProjectToListAsync<GetSurveillanceDto>(_mapper.ConfigurationProvider);

    }
}

