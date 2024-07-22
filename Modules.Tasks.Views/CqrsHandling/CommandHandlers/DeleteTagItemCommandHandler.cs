﻿using MediatR;
using Modules.Tasks.Contracts;
using Modules.Tasks.Contracts.Cqrs.Commands;
using Modules.Tasks.Contracts.Events;
using Prism.Events;

namespace Modules.Tasks.Views.CqrsHandling.CommandHandlers;

public class DeleteTagItemCommandHandler : IRequestHandler<DeleteTagItemCommand>
{
    private readonly IEventAggregator _eventAggregator;
    private readonly ITagItemRepository _tagItemRepository;

    public DeleteTagItemCommandHandler(
        IEventAggregator eventAggregator,
        ITagItemRepository tagItemRepository)
    {
        ArgumentNullException.ThrowIfNull(eventAggregator);
        ArgumentNullException.ThrowIfNull(tagItemRepository);
        
        _eventAggregator = eventAggregator;
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

        _eventAggregator.GetEvent<DeleteTagItemRequestedEvent>().Publish(request.TagId);

        return Task.CompletedTask;
    }
}
