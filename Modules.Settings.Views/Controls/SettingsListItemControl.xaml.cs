using System.Windows;
using System.Windows.Controls;

namespace Modules.Settings.Views.Controls;
/// <summary>
/// Interaction logic for SettingsListItemControl.xaml
/// </summary>
public partial class SettingsListItemControl : UserControl
{
    public static readonly DependencyProperty ActiveCategoryIdProperty =
        DependencyProperty.Register(nameof(ActiveCategoryId), typeof(int), typeof(SettingsListItemControl), new PropertyMetadata(-1));

    public int ActiveCategoryId
    {
        get => (int)GetValue(ActiveCategoryIdProperty);
        set => SetValue(ActiveCategoryIdProperty, value);
    }

    public SettingsListItemControl()
    {
        InitializeComponent();
    }
}
