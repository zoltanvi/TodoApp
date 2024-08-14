using MediatR;
using Modules.Common.Navigation;
using Modules.Common.ViewModel;
using Modules.Tasks.Contracts;
using Modules.Tasks.Contracts.Models;
using Modules.Tasks.Views.Controls;
using Modules.Tasks.Views.Events;
using Modules.Tasks.Views.Mappings;
using Prism.Events;
using PropertyChanged;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Modules.Tasks.Views.Pages;

[AddINotifyPropertyChangedInterface]
public class TaskHistoryPageViewModel : BaseViewModel, IParameterReceiver, ICloseRequester
{
    private readonly ITaskItemRepository _taskItemRepository;
    private readonly IMediator _mediator;
    private readonly IEventAggregator _eventAggregator;
    private int _taskId;
    private TaskItem? _taskItem;

    public TaskHistoryPageViewModel(
        ITaskItemRepository taskItemRepository,
        IMediator mediator,
        IEventAggregator eventAggregator)
    {
        ArgumentNullException.ThrowIfNull(taskItemRepository);
        ArgumentNullException.ThrowIfNull(mediator);
        ArgumentNullException.ThrowIfNull(eventAggregator);

        _taskItemRepository = taskItemRepository;
        _mediator = mediator;
        _eventAggregator = eventAggregator;

        _eventAggregator.GetEvent<TaskItemVersionRestoredEvent>().Subscribe(OnVersionRestored);
    }

    private void OnVersionRestored(int taskId)
    {
        if (_taskId != taskId) return;

        var dbTask = _taskItemRepository.GetTaskById(_taskId);
        ArgumentNullException.ThrowIfNull(dbTask);

        _taskItem = dbTask;

        var current = new TaskItemVersionViewModel(
            _mediator, 
            _taskItem.IsContentPlainText, 
            _taskItem.Content)
        {
            Id = -1,
            TaskId = _taskId,
            VersionDate = _taskItem.ModificationDate
        };

        var history = _taskItemRepository.GetTaskItemVersions(_taskId)
            .MapToViewModelList(_mediator)
            .OrderByDescending(x => x.VersionDate);

        HistoryItems = new ObservableCollection<TaskItemVersionViewModel>(history);
        CurrentItemList = [current];
    }

    public void ReceiveParameter(object parameter)
    {
        if (parameter is int taskId)
        {
            _taskId = taskId;
            var dbTask = _taskItemRepository.GetTaskById(_taskId);
            ArgumentNullException.ThrowIfNull(dbTask);

            _taskItem = dbTask;
            var current = new TaskItemVersionViewModel(
                _mediator,
                _taskItem.IsContentPlainText,
                _taskItem.Content)
            {
                Id = -1,
                TaskId = _taskId,
                VersionDate = _taskItem.ModificationDate
            };

            var history = _taskItemRepository.GetTaskItemVersions(_taskId)
                .MapToViewModelList(_mediator)
                .OrderByDescending(x => x.VersionDate);

            HistoryItems = new ObservableCollection<TaskItemVersionViewModel>(history);
            CurrentItemList = [current];
        }
    }

    public ObservableCollection<TaskItemVersionViewModel> HistoryItems { get; set; }

    public ObservableCollection<TaskItemVersionViewModel> CurrentItemList { get; set; }
    public ICommand ClosePageCommand { get; set; }

    protected override void OnDispose()
    {
        _eventAggregator.GetEvent<TaskItemVersionRestoredEvent>().Unsubscribe(OnVersionRestored);
    }
}
