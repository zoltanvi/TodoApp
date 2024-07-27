using Modules.Common.Navigation;
using Modules.Common.ViewModel;
using Modules.Tasks.Contracts;
using Modules.Tasks.Contracts.Models;
using Modules.Tasks.Views.Controls;
using Modules.Tasks.Views.Mappings;
using PropertyChanged;
using System.Collections.ObjectModel;

namespace Modules.Tasks.Views.Pages;

[AddINotifyPropertyChangedInterface]
public class TaskHistoryPageViewModel : BaseViewModel, IParameterReceiver
{
    private readonly ITaskItemRepository _taskItemRepository;
    private int _taskId;
    private TaskItem? _taskItem;
    
    public TaskHistoryPageViewModel(ITaskItemRepository taskItemRepository)
    {
        ArgumentNullException.ThrowIfNull(taskItemRepository);
        _taskItemRepository = taskItemRepository;
    }

    public void ReceiveParameter(object parameter)
    {
        if (parameter is int taskId)
        {
            _taskId = taskId;
            var dbTask = _taskItemRepository.GetTaskById(_taskId);
            ArgumentNullException.ThrowIfNull(dbTask);

            _taskItem = dbTask;
            CurrentContent = _taskItem.Content;

            var history = _taskItemRepository.GetTaskItemVersions(_taskId)
                .MapToViewModelList()
                .OrderByDescending(x => x.VersionDate);

            HistoryItems = new ObservableCollection<TaskItemVersionViewModel>(history);
        }
    }

    public string CurrentContent { get; private set; }

    public ObservableCollection<TaskItemVersionViewModel> HistoryItems { get; private set; }
}
