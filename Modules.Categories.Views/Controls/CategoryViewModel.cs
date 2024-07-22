using Modules.Categories.Views.Events;
using Modules.Common.DataBinding;
using Modules.Common.ViewModel;
using Prism.Events;
using PropertyChanged;
using System.Windows.Input;

namespace Modules.Categories.Views.Controls;

[AddINotifyPropertyChangedInterface]
public class CategoryViewModel : BaseViewModel, IEquatable<CategoryViewModel>
{
    public CategoryViewModel(IEventAggregator eventAggregator)
    {
        ArgumentNullException.ThrowIfNull(eventAggregator);

        DeleteCategoryCommand = new RelayCommand(() => eventAggregator.GetEvent<CategoryDeleteClickedEvent>().Publish(Id));
        ChangeCategoryCommand = new RelayCommand(() => eventAggregator.GetEvent<CategoryClickedEvent>().Publish(Id));
    }

    public int Id { get; set; }
    public required string Name { get; set; }
    public int ListOrder { get; set; }
    public DateTime CreationDate { get; set; }
    public DateTime ModificationDate { get; set; }
    public bool IsDeleted { get; set; }
    public ICommand DeleteCategoryCommand { get; }
    public ICommand ChangeCategoryCommand { get; }

    public bool Equals(CategoryViewModel? other)
    {
        return other != null && other.Id == Id;
    }
}