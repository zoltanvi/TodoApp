using MediatR;
using Modules.Common.DataBinding;
using Modules.Common.DataModels;
using Modules.Common.ViewModel;
using Modules.PopupMessage.Contracts.Cqrs.Commands;
using Modules.Settings.Views.Mappings;
using Modules.Settings.Views.Tag;
using Modules.Tasks.Contracts;
using Modules.Tasks.Contracts.Events;
using Modules.Tasks.Contracts.Models;
using Prism.Events;
using PropertyChanged;
using System.Collections.ObjectModel;
using System.Windows.Input;

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

        AddNewTagCommand = new RelayCommand(AddTag);

        var tags = _tagItemRepository.GetTags();
        Items = new ObservableCollection<TagItemViewModel>(tags.MapToViewModelList(_mediator));

        _eventAggregator.GetEvent<DeleteTagItemRequestedEvent>().Subscribe(OnDeleteTagItemRequested);
        _eventAggregator.GetEvent<TagItemUpdatedEvent>().Subscribe(OnTagItemUpdated);
    }

    public TagPresetColor SelectedColor { get; set; }

    public string PendingAddNewTagText { get; set; } = string.Empty;

    public ICommand AddNewTagCommand { get; set; }

    public ObservableCollection<TagItemViewModel> Items { get; }

    private void AddTag()
    {
        if (string.IsNullOrWhiteSpace(PendingAddNewTagText)) return;

        var tagItem = Items.FirstOrDefault(x => x.Name.Equals(PendingAddNewTagText));
        if (tagItem != null)
        {
            _mediator.Send(new ShowMessageErrorCommand { Message = "A tag with this name already exists!" });
            return;
        }

        var dbTag = _tagItemRepository.AddTag(new TagItem
        {
            Name = PendingAddNewTagText,
            Color = SelectedColor.ToString(),
        });

        Items.Add(dbTag.MapToViewModel(_mediator));

        PendingAddNewTagText = string.Empty;
    }

    private void OnDeleteTagItemRequested(int tagId)
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

    protected override void OnDispose()
    {
        _eventAggregator.GetEvent<DeleteTagItemRequestedEvent>().Unsubscribe(OnDeleteTagItemRequested);
        _eventAggregator.GetEvent<TagItemUpdatedEvent>().Unsubscribe(OnTagItemUpdated);
    }
}
