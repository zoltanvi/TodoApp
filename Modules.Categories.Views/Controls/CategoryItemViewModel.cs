using Modules.Categories.Views.Events;
using Modules.Common.DataBinding;
using Modules.Common.ViewModel;
using Prism.Events;
using PropertyChanged;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Modules.Categories.Views.Controls;

[AddINotifyPropertyChangedInterface]
public class CategoryItemViewModel : BaseViewModel, IEquatable<CategoryItemViewModel>
{
    public CategoryItemViewModel(IEventAggregator eventAggregator)
    {
        ArgumentNullException.ThrowIfNull(eventAggregator);

        DeleteCategoryCommand = new RelayCommand(() => eventAggregator.GetEvent<CategoryDeleteClickedEvent>().Publish(Id));
        ChangeCategoryCommand = new RelayCommand(() => eventAggregator.GetEvent<CategoryClickedEvent>().Publish(Id));
        ToggleExpandCommand = new RelayCommand(() => eventAggregator.GetEvent<CategoryToggleExpandEvent>().Publish(Id));
        AddSubcategoryCommand = new RelayCommand(() => eventAggregator.GetEvent<CategoryAddSubcategoryClickedEvent>().Publish(Id));
        RenameCommand = new RelayCommand(() => eventAggregator.GetEvent<CategoryRenameClickedEvent>().Publish(Id));
        MoveToRootCommand = new RelayCommand(() => eventAggregator.GetEvent<CategoryMoveToRootClickedEvent>().Publish(Id));
    }

    public int Id { get; set; }
    public int? ParentCategoryId { get; set; }
    public required string Name { get; set; }
    public int ListOrder { get; set; }
    public DateTime CreationDate { get; set; }
    public DateTime ModificationDate { get; set; }
    public DateTime? DeletedDate { get; set; }

    public bool IsDeleted => DeletedDate.HasValue;

    public ObservableCollection<CategoryItemViewModel> Children { get; set; } = [];
    public bool HasChildren { get; set; }
    public bool IsExpanded { get; set; }
    public int Depth { get; set; }
    public bool IsRenaming { get; set; }
    public string? RenameText { get; set; }

    public ICommand DeleteCategoryCommand { get; }
    public ICommand ChangeCategoryCommand { get; }
    public ICommand ToggleExpandCommand { get; }
    public ICommand AddSubcategoryCommand { get; }
    public ICommand RenameCommand { get; }
    public ICommand MoveToRootCommand { get; }

    public bool IsSubcategory => ParentCategoryId != null;

    public bool Equals(CategoryItemViewModel? other)
    {
        return other != null && other.Id == Id;
    }
}