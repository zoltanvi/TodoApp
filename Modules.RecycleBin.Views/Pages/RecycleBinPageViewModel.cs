using MediatR;
using Modules.Categories.Contracts;
using Modules.Categories.Contracts.Cqrs.Events;
using Modules.Common.ViewModel;
using Modules.RecycleBin.Repositories;
using Modules.RecycleBin.Views.Controls;
using Modules.RecycleBin.Views.Mappings;
using Modules.Tasks.Contracts.Cqrs.Events;
using Modules.Tasks.Contracts.Models;
using PropertyChanged;
using System.Collections.ObjectModel;

namespace Modules.RecycleBin.Views.Pages;

[AddINotifyPropertyChangedInterface]
public class RecycleBinPageViewModel : BaseViewModel
{
    private readonly IMediator _mediator;
    private readonly ICategoriesRepository _categoryRepository;
    private readonly RecycleBinRepository _recycleBinRepository;

    public RecycleBinPageViewModel(
        IMediator mediator,
        ICategoriesRepository categoryRepository,
        RecycleBinRepository recycleBinRepository)
    {
        ArgumentNullException.ThrowIfNull(mediator);
        ArgumentNullException.ThrowIfNull(categoryRepository);
        ArgumentNullException.ThrowIfNull(recycleBinRepository);

        _mediator = mediator;
        _categoryRepository = categoryRepository;
        _recycleBinRepository = recycleBinRepository;

        InitializeGroupItems();

        TaskRestoredEvent.TaskRestored += OnTaskRestored;
        CategoryDeletedEvent.CategoryDeleted += OnCategoryDeleted;
    }

    public ObservableCollection<RecycleBinGroupItemViewModel> GroupItems { get; set; } = new();
    public bool IsEmpty => GroupItems.Count == 0;

    private void OnTaskRestored(TaskRestoredEvent obj)
    {
        var group = GroupItems.FirstOrDefault(x => x.CategoryId == obj.CategoryId);
        ArgumentNullException.ThrowIfNull(group);

        var task = group.Items.First(x => x.Id == obj.TaskId);
        ArgumentNullException.ThrowIfNull(group);

        group.Items.Remove(task);

        if (group.Items.Count == 0)
        {
            GroupItems.Remove(group);
        }

        OnPropertyChanged(nameof(IsEmpty));
    }

    private void OnCategoryDeleted(CategoryDeletedEvent obj)
    {
        var deletedTasksFromCategory = _recycleBinRepository.GetDeletedTasksFromCategory(obj.CategoryId);

        if (deletedTasksFromCategory.Count == 0) return;

        var items = new ObservableCollection<RecycleBinTaskItemViewModel>();
        foreach (TaskItem item in deletedTasksFromCategory)
        {
            items.Add(item.MapToRecycleBinTaskItem(_mediator));
        }

        var group = GroupItems.FirstOrDefault(x => x.CategoryId == obj.CategoryId);
        if (group == null)
        {
            var category = _categoryRepository.GetCategoryById(obj.CategoryId);
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
        TaskRestoredEvent.TaskRestored -= OnTaskRestored;
        CategoryDeletedEvent.CategoryDeleted -= OnCategoryDeleted;
    }
}
