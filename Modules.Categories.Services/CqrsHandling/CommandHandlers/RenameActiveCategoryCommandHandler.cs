using MediatR;
using Modules.Categories.Contracts.Cqrs.Commands;

namespace Modules.Categories.Services.CqrsHandling.CommandHandlers;

public class RenameActiveCategoryCommandHandler : IRequestHandler<RenameActiveCategoryCommand>
{
    public Task Handle(RenameActiveCategoryCommand request, CancellationToken cancellationToken)
    {
        // TODO: implement
        return Task.CompletedTask;
    }
}
