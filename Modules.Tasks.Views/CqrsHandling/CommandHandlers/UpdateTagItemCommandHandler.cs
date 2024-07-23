using MediatR;
using Modules.Tasks.Contracts;
using Modules.Tasks.Contracts.Cqrs.Commands;
using Modules.Tasks.Contracts.Events;
using Prism.Events;

namespace Modules.Tasks.Views.CqrsHandling.CommandHandlers;

public class UpdateTagItemCommandHandler : IRequestHandler<UpdateTagItemCommand>
{
    private readonly IEventAggregator _eventAggregator;
    private readonly ITagItemRepository _tagItemRepository;

    public UpdateTagItemCommandHandler(
        IEventAggregator eventAggregator,
        ITagItemRepository tagItemRepository)
    {
        ArgumentNullException.ThrowIfNull(eventAggregator);
        ArgumentNullException.ThrowIfNull(tagItemRepository);

        _eventAggregator = eventAggregator;
        _tagItemRepository = tagItemRepository;
    }

    public Task Handle(UpdateTagItemCommand request, CancellationToken cancellationToken)
    {
        var dbTag = _tagItemRepository.GetTagById(request.TagId);
        ArgumentNullException.ThrowIfNull(dbTag);

        dbTag.Name = request.NewName;
        dbTag.Color = request.Color;

        _tagItemRepository.UpdateTag(dbTag);

        _eventAggregator.GetEvent<TagItemUpdatedEvent>().Publish(request.TagId);

        return Task.CompletedTask;
    }
}
