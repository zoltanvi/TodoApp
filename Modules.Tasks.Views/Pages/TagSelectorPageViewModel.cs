using Modules.Common.Navigation;
using Modules.Common.ViewModel;
using Modules.Tasks.Contracts;
using Modules.Tasks.Contracts.Models;
using Modules.Tasks.Views.Events;
using Modules.Tasks.Views.Mappings;
using Prism.Events;

namespace Modules.Tasks.Views.Pages;

public class TagSelectorPageViewModel : BaseViewModel, IParameterReceiver
{
    private readonly ITaskItemRepository _taskItemRepository;
    private readonly ITagItemRepository _tagItemRepository;
    private int _selectedTagId;
    private int _taskId;

    public TagSelectorPageViewModel(
        ITaskItemRepository taskItemRepository,
        ITagItemRepository tagItemRepository,
        IEventAggregator eventAggregator)
    {
        ArgumentNullException.ThrowIfNull(taskItemRepository);
        ArgumentNullException.ThrowIfNull(tagItemRepository);
        ArgumentNullException.ThrowIfNull(eventAggregator);

        _taskItemRepository = taskItemRepository;
        _tagItemRepository = tagItemRepository;

        List<TagItem> tags = _tagItemRepository.GetTags();
        Items = new List<TagSelectionItemViewModel>(tags.MapToViewModelList(eventAggregator));
        eventAggregator.GetEvent<TagSelectionItemClickedEvent>().Subscribe(SelectTag);
    }

    public List<TagSelectionItemViewModel> Items { get; }

    public int SelectedTagId
    {
        get => _selectedTagId;
        set
        {
            _selectedTagId = value;

            var dbTask = _taskItemRepository.GetTaskById(_taskId);
            ArgumentNullException.ThrowIfNull(dbTask);

            var dbTag = _tagItemRepository.GetTagById(_selectedTagId);
            ArgumentNullException.ThrowIfNull(dbTag);

            _taskItemRepository.RemoveTagsFromTask(dbTask);
            _taskItemRepository.AddTagToTask(dbTask, dbTag);
        }
    }

    public void ReceiveParameter(object parameter)
    {
        if (parameter is int taskId)
        {
            _taskId = taskId;
        }
    }

    private void SelectTag(int id)
    {
        SelectedTagId = id;

        foreach (var tagVm in Items)
        {
            if (tagVm.Id != id)
            {
                tagVm.IsSelected = false;
            }
        }
    }
}
