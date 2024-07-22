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
        _eventAggregator.GetEvent<TagSelectedEvent>().Subscribe(SelectTag);
        _eventAggregator.GetEvent<TagDeselectedEvent>().Subscribe(DeselectTag);
    }

    public List<TagSelectionItemViewModel> Items { get; }

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
