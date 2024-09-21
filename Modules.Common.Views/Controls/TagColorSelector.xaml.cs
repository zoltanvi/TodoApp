using Modules.Common.DataModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Modules.Common.Views.Controls;

public partial class TagColorSelector : UserControl
{
    public static readonly DependencyProperty SelectedColorProperty = DependencyProperty.Register(
        nameof(SelectedColor), typeof(TagColor), typeof(TagColorSelector), new PropertyMetadata(default(TagColor)));

    public TagColor SelectedColor
    {
        get => (TagColor)GetValue(SelectedColorProperty);
        set => SetValue(SelectedColorProperty, value);
    }

    public TagColorSelector()
    {
        InitializeComponent();
    }

    private void ToggleButton_OnChecked(object sender, RoutedEventArgs e)
    {
        if (sender is ToggleButton toggleButton &&
            toggleButton.Tag is TagColor colorToSelect)
        {
            SelectedColor = colorToSelect;
        }
    }
}
