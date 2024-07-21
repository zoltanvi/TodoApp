using MediatR;
using Modules.Common.DataBinding;
using Modules.Common.DataModels;
using Modules.Common.ViewModel;
using Modules.PopupMessage.Contracts.Cqrs.Commands;
using Modules.Settings.Views.Mappings;
using Modules.Settings.Views.Tag;
using Modules.Tasks.Contracts;
using Modules.Tasks.Contracts.Cqrs.Events;
using Modules.Tasks.Contracts.Models;
using PropertyChanged;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Modules.Settings.Views.Pages;

[AddINotifyPropertyChangedInterface]
public class TagSettingsPageViewModel : BaseViewModel
{
    private readonly IMediator _mediator;
    private readonly ITagItemRepository _tagItemRepository;

    public TagSettingsPageViewModel(
        IMediator mediator,
        ITagItemRepository tagItemRepository)
    {
        ArgumentNullException.ThrowIfNull(mediator);
        ArgumentNullException.ThrowIfNull(tagItemRepository);
        
        _mediator = mediator;
        _tagItemRepository = tagItemRepository;

        AddNewTagCommand = new RelayCommand(AddTag);

        var tags = _tagItemRepository.GetTags();
        Items = new ObservableCollection<TagItemViewModel>(tags.MapToViewModelList(_mediator));

        DeleteTagItemRequestedEvent.DeleteTagItemRequested += OnDeleteTagItemRequested;
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

    private void OnDeleteTagItemRequested(DeleteTagItemRequestedEvent obj)
    {
        var tag = Items.FirstOrDefault(x => x.Id == obj.TagId);
        ArgumentNullException.ThrowIfNull(tag);
        
        Items.Remove(tag);
    }

    protected override void OnDispose()
    {
        DeleteTagItemRequestedEvent.DeleteTagItemRequested -= OnDeleteTagItemRequested;
    }
}
