using MediatR;
using Modules.Categories.Contracts;
using Modules.Common.DataBinding;
using Modules.Common.ViewModel;
using Modules.RecycleBin.Repositories;
using Modules.Tasks.Contracts.Cqrs.Commands;
using Modules.Tasks.Contracts.Models;
using PropertyChanged;
using System.Collections.ObjectModel;
using System.Windows.Input;

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

        var deletedTasksGroupByCategory = _recycleBinRepository.GetDeletedTasksGroupByCategory();

        foreach (IGrouping<int, TaskItem> grouping in deletedTasksGroupByCategory)
        {
            var items = new ObservableCollection<RecycleBinTaskItemViewModel>();

            foreach (TaskItem item in grouping)
            {
                items.Add(new RecycleBinTaskItemViewModel(_mediator)
                {
                    Id = item.Id,
                    CategoryId = item.CategoryId,
                    Content = item.Content,
                    ContentPreview = item.ContentPreview,
                    Pinned = item.Pinned,
                    IsDone = item.IsDone,
                    BackgroundColor = item.BackgroundColor,
                    BorderColor = item.BorderColor,
                    CreationDate = item.CreationDate,
                    DeletedDate = item.DeletedDate,
                    IsDeleted = item.IsDeleted,
                    ListOrder = item.ListOrder,
                    MarkerColor = item.MarkerColor,
                    ModificationDate = item.ModificationDate
                });
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
    }

    public ObservableCollection<RecycleBinGroupItemViewModel> GroupItems { get; set; } = new();
}

public class RecycleBinGroupItemViewModel : BaseViewModel
{
    public int CategoryId { get; set; }
    public string CategoryName { get; set; }
    public ObservableCollection<RecycleBinTaskItemViewModel> Items { get; set; }
}

public class RecycleBinTaskItemViewModel : BaseViewModel
{
    private readonly IMediator _mediator;
    public int Id { get; set; }
    public required int CategoryId { get; set; }
    public required string Content { get; set; }
    public required string ContentPreview { get; set; }
    public int ListOrder { get; set; }
    public bool Pinned { get; set; }
    public bool IsDone { get; set; }
    public DateTime CreationDate { get; set; }
    public DateTime ModificationDate { get; set; }
    public string? MarkerColor { get; set; }
    public string? BorderColor { get; set; }
    public string? BackgroundColor { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedDate { get; set; }
    public bool DetailsVisible { get; set; }
    public ICommand ToggleDetailsCommand { get; }
    public ICommand RestoreTaskItemCommand { get; }

    public RecycleBinTaskItemViewModel(IMediator mediator)
    {
        ArgumentNullException.ThrowIfNull(mediator);

        _mediator = mediator;

        ToggleDetailsCommand = new RelayCommand(() => DetailsVisible ^= true);
        RestoreTaskItemCommand = new RelayCommand(() => _mediator.Send(new RestoreTaskItemCommand { TaskId = Id }));
    }
}
