using MediatR;
using Modules.Common.ViewModel;
using Modules.Common.Views.Controls;
using Modules.Settings.Views.Mappings;
using Modules.Settings.Views.Tag;
using Modules.Tasks.Contracts;
using Modules.Tasks.Contracts.Events;
using Prism.Events;
using PropertyChanged;
using System.Collections.ObjectModel;

namespace Modules.Settings.Views.Pages;

[AddINotifyPropertyChangedInterface]
public class TagSettingsPageViewModel : BaseViewModel
{
    private readonly IMediator _mediator;
    private readonly ITagItemRepository _tagItemRepository;
    private readonly IEventAggregator _eventAggregator;

    public TagSettingsPageViewModel(
        IMediator mediator,
        ITagItemRepository tagItemRepository,
        IEventAggregator eventAggregator)
    {
        ArgumentNullException.ThrowIfNull(mediator);
        ArgumentNullException.ThrowIfNull(tagItemRepository);
        ArgumentNullException.ThrowIfNull(eventAggregator);

        _mediator = mediator;
        _tagItemRepository = tagItemRepository;
        _eventAggregator = eventAggregator;

        var tags = _tagItemRepository.GetTags();
        Items = new ObservableCollection<TagItemViewModel>(tags.MapToViewModelList(_mediator));
        TagCreator = new TagCreatorViewModel(_tagItemRepository, _mediator, _eventAggregator);

        _eventAggregator.GetEvent<TagItemCreatedEvent>().Subscribe(OnTagItemCreated);
        _eventAggregator.GetEvent<TagItemUpdatedEvent>().Subscribe(OnTagItemUpdated);
        _eventAggregator.GetEvent<TagItemDeletedEvent>().Subscribe(OnTagItemDeleted);
    }

    public ObservableCollection<TagItemViewModel> Items { get; }
    public TagCreatorViewModel TagCreator { get; set; }

    private void OnTagItemDeleted(int tagId)
    {
        var tag = Items.FirstOrDefault(x => x.Id == tagId);
        ArgumentNullException.ThrowIfNull(tag);
        
        Items.Remove(tag);
    }

    private void OnTagItemUpdated(int tagId)
    {
        var tag = Items.FirstOrDefault(x => x.Id == tagId);
        ArgumentNullException.ThrowIfNull(tag);

        var dbTag = _tagItemRepository.GetTagById(tagId);
        ArgumentNullException.ThrowIfNull(dbTag);
        
        var index = Items.IndexOf(tag);
        Items.RemoveAt(index);
        Items.Insert(index, dbTag.MapToViewModel(_mediator));
    }

    private void OnTagItemCreated(int tagId)
    {
        var dbTag = _tagItemRepository.GetTagById(tagId);
        ArgumentNullException.ThrowIfNull(dbTag);
        
        Items.Add(dbTag.MapToViewModel(_mediator));
    }

    protected override void OnDispose()
    {
        _eventAggregator.GetEvent<TagItemCreatedEvent>().Unsubscribe(OnTagItemCreated);
        _eventAggregator.GetEvent<TagItemUpdatedEvent>().Unsubscribe(OnTagItemUpdated);
        _eventAggregator.GetEvent<TagItemDeletedEvent>().Unsubscribe(OnTagItemDeleted);
    }
}
