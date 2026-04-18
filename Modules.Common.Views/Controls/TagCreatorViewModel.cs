using Modules.Common.DataBinding;
using Modules.Common.DataModels;
using Modules.Common.ViewModel;
using Modules.PopupMessage.Contracts;
using Modules.Tasks.Contracts;
using Modules.Tasks.Contracts.Events;
using Modules.Tasks.Contracts.Models;
using Prism.Events;
using PropertyChanged;
using System.Windows.Input;

namespace Modules.Common.Views.Controls;

[AddINotifyPropertyChangedInterface]
public class TagCreatorViewModel : BaseViewModel
{
    private readonly ITagItemRepository _tagItemRepository;
    private readonly IPopupMessageService _popupMessageService;
    private readonly IEventAggregator _eventAggregator;

    public TagCreatorViewModel(
        ITagItemRepository tagItemRepository,
        IPopupMessageService popupMessageService,
        IEventAggregator eventAggregator)
    {
        ArgumentNullException.ThrowIfNull(tagItemRepository);
        ArgumentNullException.ThrowIfNull(popupMessageService);
        ArgumentNullException.ThrowIfNull(eventAggregator);

        _tagItemRepository = tagItemRepository;
        _popupMessageService = popupMessageService;
        _eventAggregator = eventAggregator;

        AddNewTagCommand = new RelayCommand(AddTag);
    }

    public TagColor SelectedColor { get; set; }

    public string PendingAddNewTagText { get; set; } = string.Empty;

    public ICommand AddNewTagCommand { get; set; }

    private void AddTag()
    {
        if (string.IsNullOrWhiteSpace(PendingAddNewTagText)) return;

        var tagItem = _tagItemRepository.GetTagByName(PendingAddNewTagText);
        if (tagItem != null)
        {
            _popupMessageService.ShowError("A tag with this name already exists!");
            return;
        }

        var dbTag = _tagItemRepository.AddTag(new TagItem
        {
            Name = PendingAddNewTagText,
            Color = SelectedColor.ToString(),
        });

        _eventAggregator.GetEvent<TagItemCreatedEvent>().Publish(dbTag.Id);

        PendingAddNewTagText = string.Empty;
    }
}
