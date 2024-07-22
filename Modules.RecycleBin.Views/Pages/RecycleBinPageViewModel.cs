using MediatR;
using Modules.Categories.Contracts;
using Modules.Categories.Contracts.Events;
using Modules.Common.ViewModel;
using Modules.RecycleBin.Repositories;
using Modules.RecycleBin.Views.Controls;
using Modules.RecycleBin.Views.Mappings;
using Modules.Tasks.Contracts.Events;
using Modules.Tasks.Contracts.Models;
using Prism.Events;
using PropertyChanged;
using System.Collections.ObjectModel;

namespace Modules.RecycleBin.Views.Pages;

[AddINotifyPropertyChangedInterface]
public class RecycleBinPageViewModel : BaseViewModel
{
    private readonly IMediator _mediator;
    private readonly IEventAggregator _eventAggregator;
    private readonly ICategoriesRepository _categoryRepository;
    private readonly RecycleBinRepository _recycleBinRepository;

    public RecycleBinPageViewModel(
        IMediator mediator,
        IEventAggregator eventAggregator,
        ICategoriesRepository categoryRepository,
        RecycleBinRepository recycleBinRepository)
    {
        ArgumentNullException.ThrowIfNull(mediator);
        ArgumentNullException.ThrowIfNull(eventAggregator);
        ArgumentNullException.ThrowIfNull(categoryRepository);
        ArgumentNullException.ThrowIfNull(recycleBinRepository);

        _mediator = mediator;
        _eventAggregator = eventAggregator;
        _categoryRepository = categoryRepository;
        _recycleBinRepository = recycleBinRepository;

        InitializeGroupItems();

        _eventAggregator.GetEvent<CategoryDeletedEvent>().Subscribe(OnCategoryDeleted);
        _eventAggregator.GetEvent<TaskRestoredEvent>().Subscribe(OnTaskRestored);
    }

    public ObservableCollection<RecycleBinGroupItemViewModel> GroupItems { get; } = new();
    public bool IsEmpty => GroupItems.Count == 0;

    private void OnTaskRestored(TaskRestoredPayload payload)
    {
        var group = GroupItems.FirstOrDefault(x => x.CategoryId == payload.CategoryId);
        ArgumentNullException.ThrowIfNull(group);

        var task = group.Items.First(x => x.Id == payload.TaskId);
        ArgumentNullException.ThrowIfNull(group);

        group.Items.Remove(task);

        if (group.Items.Count == 0)
        {
            GroupItems.Remove(group);
        }

        OnPropertyChanged(nameof(IsEmpty));
    }

    private void OnCategoryDeleted(int categoryId)
    {
        var deletedTasksFromCategory = _recycleBinRepository.GetDeletedTasksFromCategory(categoryId);

        if (deletedTasksFromCategory.Count == 0) return;

        var items = new ObservableCollection<RecycleBinTaskItemViewModel>();
        foreach (TaskItem item in deletedTasksFromCategory)
        {
            items.Add(item.MapToRecycleBinTaskItem(_mediator));
        }

        var group = GroupItems.FirstOrDefault(x => x.CategoryId == categoryId);
        if (group == null)
        {
            var category = _categoryRepository.GetCategoryById(categoryId);
            ArgumentNullException.ThrowIfNull(category);

            GroupItems.Add(new RecycleBinGroupItemViewModel
            {
                CategoryId = category.Id,
                CategoryName = category.Name,
                Items = items
            });
        }
        else
        {
            group.Items.Clear();
            foreach (var item in items)
            {
                group.Items.Add(item);
            }   
        }

        OnPropertyChanged(nameof(IsEmpty));
    }

    private void InitializeGroupItems()
    {
        var deletedTasksGroupByCategory = _recycleBinRepository.GetDeletedTasksGroupByCategory();

        foreach (IGrouping<int, TaskItem> grouping in deletedTasksGroupByCategory)
        {
            var items = new ObservableCollection<RecycleBinTaskItemViewModel>();

            foreach (TaskItem item in grouping)
            {
                items.Add(item.MapToRecycleBinTaskItem(_mediator));
            }

            var category = _categoryRepository.GetCategoryById(grouping.Key);
            ArgumentNullException.ThrowIfNull(category);

            GroupItems.Add(new RecycleBinGroupItemViewModel
            {
                CategoryId = category.Id,
                CategoryName = category.Name,
                Items = items
            });
        }

        OnPropertyChanged(nameof(IsEmpty));
    }

    protected override void OnDispose()
    {
        _eventAggregator.GetEvent<TaskRestoredEvent>().Unsubscribe(OnTaskRestored);
        _eventAggregator.GetEvent<CategoryDeletedEvent>().Unsubscribe(OnCategoryDeleted);
    }
}
