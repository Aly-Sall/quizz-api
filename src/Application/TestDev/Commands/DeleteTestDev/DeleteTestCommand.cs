// src/Application/TestDev/Commands/DeleteTestDev/DeleteTestCommand.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _Net6CleanArchitectureQuizzApp.Application.Common.Exceptions;
using _Net6CleanArchitectureQuizzApp.Application.Common.Interfaces;
using _Net6CleanArchitectureQuizzApp.Domain.Entities;
using MediatR;

namespace _Net6CleanArchitectureQuizzApp.Application.TestDev.Commands.DeleteTestDev;

public record DeleteTestCommand(int Id) : IRequest;

public class DeleteTestCommandHandler : IRequestHandler<DeleteTestCommand>
{
    private readonly IApplicationDbContext _context;

    public DeleteTestCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(DeleteTestCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Tests
            .FindAsync(new object[] { request.Id }, cancellationToken);

        if (entity == null)
        {
            throw new NotFoundException(nameof(QuizTest), request.Id);
        }

        // Vérifier si le test est actif - on ne peut pas supprimer un test actif
        if (entity.IsActive)
        {
            throw new ForbiddenAccessException();
        }

        _context.Tests.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}