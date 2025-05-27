using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _Net6CleanArchitectureQuizzApp.Application.Common.Exceptions;
using _Net6CleanArchitectureQuizzApp.Application.Common.Interfaces;
using _Net6CleanArchitectureQuizzApp.Domain.Entities;
using _Net6CleanArchitectureQuizzApp.Domain.Enums;
using MediatR;

namespace _Net6CleanArchitectureQuizzApp.Application.TestDev.Commands.UpdateTestDev;

public record UpdateTestCommand : IRequest
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public Category Category { get; set; }
    public Mode Mode { get; set; }
    public bool ShowTimer { get; set; } 


}
public class UpdateTestCommandHandler : IRequestHandler<UpdateTestCommand>
{
    private readonly IApplicationDbContext _context;

    public UpdateTestCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(UpdateTestCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Tests
            .FindAsync(new object[] { request.Id }, cancellationToken);

        if (entity == null)
        {
            throw new NotFoundException(nameof(Question), request.Id);
        }
        if(entity.IsActive==true)
        {
            throw new ForbiddenAccessException();
        }

        entity.Title = request.Title;
        entity.ShowTimer = request.ShowTimer;
        entity.Category = request.Category;
        entity.Mode = request.Mode;


         _context.Tests.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
//public record UpdateTodoItemCommand : IRequest


//public class UpdateTodoItemCommandHandler : IRequestHandler<UpdateTodoItemCommand>
//{
//    private readonly IApplicationDbContext _context;

//    public UpdateTodoItemCommandHandler(IApplicationDbContext context)
//    {
//        _context = context;
//    }

//    public async Task<Unit> Handle(UpdateTodoItemCommand request, CancellationToken cancellationToken)
//    {
//        var entity = await _context.TodoItems
//            .FindAsync(new object[] { request.Id }, cancellationToken);

//        if (entity == null)
//        {
//            throw new NotFoundException(nameof(TodoItem), request.Id);
//        }

//        entity.Title = request.Title;
//        entity.Done = request.Done;

//        await _context.SaveChangesAsync(cancellationToken);

//        return Unit.Value;
//    }
//}
