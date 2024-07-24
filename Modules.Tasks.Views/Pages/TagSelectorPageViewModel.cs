using Modules.Common.DataBinding;
using Modules.Common.Navigation;
using Modules.Common.ViewModel;
using Modules.Tasks.Contracts;
using Modules.Tasks.Contracts.Models;
using Modules.Tasks.Views.Events;
using Modules.Tasks.Views.Mappings;
using Prism.Events;
using System.Windows.Input;

namespace Modules.Tasks.Views.Pages;

public class TagSelectorPageViewModel : BaseViewModel, IParameterReceiver
{
    private readonly ITaskItemRepository _taskItemRepository;
    private readonly ITagItemRepository _tagItemRepository;
    private readonly IEventAggregator _eventAggregator;
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
        _eventAggregator = eventAggregator;

        List<TagItem> tags = _tagItemRepository.GetTags();
        Items = new List<TagSelectionItemViewModel>(tags.MapToViewModelList(eventAggregator));

        DeselectAllTagsCommand = new RelayCommand(DeselectAllTags);

        _eventAggregator.GetEvent<TagSelectedEvent>().Subscribe(SelectTag);
        _eventAggregator.GetEvent<TagDeselectedEvent>().Subscribe(DeselectTag);
    }

    private void DeselectAllTags()
    {
        var dbTask = _taskItemRepository.GetTaskById(_taskId);
        ArgumentNullException.ThrowIfNull(dbTask);

        _taskItemRepository.RemoveTagsFromTask(dbTask);

        foreach (var item in Items)
        {
            item.IsSelected = false;
        }

        _eventAggregator.GetEvent<TagsChangedOnTaskItemEvent>().Publish(_taskId);
    }

    public List<TagSelectionItemViewModel> Items { get; }

    public ICommand DeselectAllTagsCommand { get; set; }

    public void ReceiveParameter(object parameter)
    {
        if (parameter is int taskId)
        {
            _taskId = taskId;
            var dbTask = _taskItemRepository.GetTaskById(_taskId);
            ArgumentNullException.ThrowIfNull(dbTask);

            // Set tags that are already on the task to selected
            foreach (var tagItem in dbTask.Tags)
            {
                var tag = Items.First(x => x.Id == tagItem.Id);
                tag.IsSelected = true;
            }
        }
    }

    private void SelectTag(int tagId)
    {
        var dbTask = _taskItemRepository.GetTaskById(_taskId);
        ArgumentNullException.ThrowIfNull(dbTask);

        var tag = Items.First(x => x.Id == tagId);

        _taskItemRepository.AddTagToTask(dbTask, tag.Map());

        _eventAggregator.GetEvent<TagsChangedOnTaskItemEvent>().Publish(_taskId);
    }

    private void DeselectTag(int tagId)
    {
        var dbTask = _taskItemRepository.GetTaskById(_taskId);
        ArgumentNullException.ThrowIfNull(dbTask);

        var tag = Items.First(x => x.Id == tagId);

        _taskItemRepository.RemoveTagFromTask(dbTask, tag.Map());

        _eventAggregator.GetEvent<TagsChangedOnTaskItemEvent>().Publish(_taskId);
    }

    protected override void OnDispose()
    {
        _eventAggregator.GetEvent<TagSelectedEvent>().Unsubscribe(SelectTag);
        _eventAggregator.GetEvent<TagDeselectedEvent>().Unsubscribe(DeselectTag);
    }
}
