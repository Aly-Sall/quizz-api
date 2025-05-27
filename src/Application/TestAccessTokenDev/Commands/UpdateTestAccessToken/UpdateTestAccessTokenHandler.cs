using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _Net6CleanArchitectureQuizzApp.Application.Common.Exceptions;
using _Net6CleanArchitectureQuizzApp.Application.Common.Interfaces;
using _Net6CleanArchitectureQuizzApp.Application.Common.Models;
using _Net6CleanArchitectureQuizzApp.Application.TestAccessTokenDev.Commands.GenerateTestAccessToken;
using _Net6CleanArchitectureQuizzApp.Domain.Entities;
using FluentValidation;
using MediatR;

namespace _Net6CleanArchitectureQuizzApp.Application.TestAccessTokenDev.Commands.UpdateTestAccessToken;
public class UpdateTestAccessTokenHandler : IRequestHandler<UpdateTestAccessTokenCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly IValidator<UpdateTestAccessTokenCommand> _validator;


    public UpdateTestAccessTokenHandler(IApplicationDbContext context, IValidator<UpdateTestAccessTokenCommand> validator)
    {
        _context = context;
        _validator = validator;
    }
    public async Task<Unit> Handle(UpdateTestAccessTokenCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.TestAccessTokens
                    .FindAsync(new object[] { request.Id }, cancellationToken);

        if (entity == null)
        {
            throw new NotFoundException(nameof(Question), request.Id);
        }
        if (entity.IsUsed == true)
        {
            throw new ForbiddenAccessException();
        }

        entity.IsUsed = request.IsUsed;
        entity.TentativeId = request.TentativeId;

        _context.TestAccessTokens.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}

