using MediatR;
using Modules.Tasks.Contracts;
using Modules.Tasks.Contracts.Cqrs.Commands;
using Modules.Tasks.Contracts.Cqrs.Events;

namespace Modules.Tasks.Views.CqrsHandling.CommandHandlers;

public class DeleteTagItemCommandHandler : IRequestHandler<DeleteTagItemCommand>
{
    private readonly ITagItemRepository _tagItemRepository;

    public DeleteTagItemCommandHandler(ITagItemRepository tagItemRepository)
    {
        ArgumentNullException.ThrowIfNull(tagItemRepository);
        _tagItemRepository = tagItemRepository;
    }

    public Task Handle(DeleteTagItemCommand request, CancellationToken cancellationToken)
    {
        var dbTag = _tagItemRepository.GetTagById(request.TagId);
        ArgumentNullException.ThrowIfNull(dbTag);

        // Remove
        //foreach (TaskItem taskItem in dbTag.TaskItems)
        //{
        //    taskItem.Tags.Remove(dbTag);
        //}

        _tagItemRepository.DeleteTag(dbTag);

        DeleteTagItemRequestedEvent.Invoke(new DeleteTagItemRequestedEvent { TagId = request.TagId });

        return Task.CompletedTask;
    }
}
