using _Net6CleanArchitectureQuizzApp.Application.Common.Exceptions;
using _Net6CleanArchitectureQuizzApp.Application.Common.Interfaces;
using _Net6CleanArchitectureQuizzApp.Domain.Entities;
using MediatR;

namespace _Net6CleanArchitectureQuizzApp.Application.TodoItems.Commands.DeleteTodoItem;

//public record DeleteTodoItemCommand(int Id) : IRequest;

//public class DeleteTodoItemCommandHandler : IRequestHandler<DeleteTodoItemCommand>
//{
//    private readonly IApplicationDbContext _context;

//    public DeleteTodoItemCommandHandler(IApplicationDbContext context)
//    {
//        _context = context;
//    }

//    public async Task<Unit> Handle(DeleteTodoItemCommand request, CancellationToken cancellationToken)
//    {
//        var entity = await _context.TodoItems
//            .FindAsync(new object[] { request.Id }, cancellationToken);

//        if (entity == null)
//        {
//            throw new NotFoundException(nameof(TodoItem), request.Id);
//        }

//        _context.TodoItems.Remove(entity);

//        entity.AddDomainEvent(new TodoItemDeletedEvent(entity));

//        await _context.SaveChangesAsync(cancellationToken);

//        return Unit.Value;
//    }
//}
