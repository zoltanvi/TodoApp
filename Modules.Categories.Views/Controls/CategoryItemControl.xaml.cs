using System.Windows;
using System.Windows.Controls;

namespace Modules.Categories.Views.Controls;

public partial class CategoryItemControl : UserControl
{
    public static readonly DependencyProperty ActiveCategoryIdProperty =
       DependencyProperty.Register(nameof(ActiveCategoryId), typeof(int), typeof(CategoryItemControl), new PropertyMetadata(-1));

    public static readonly DependencyProperty FocusedCategoryIdProperty =
       DependencyProperty.Register(nameof(FocusedCategoryId), typeof(int), typeof(CategoryItemControl), new PropertyMetadata(-1));

    public int ActiveCategoryId
    {
        get => (int)GetValue(ActiveCategoryIdProperty);
        set => SetValue(ActiveCategoryIdProperty, value);
    }

    public int FocusedCategoryId
    {
        get => (int)GetValue(FocusedCategoryIdProperty);
        set => SetValue(FocusedCategoryIdProperty, value);
    }

    public CategoryItemControl()
    {
        InitializeComponent();
    }
}
