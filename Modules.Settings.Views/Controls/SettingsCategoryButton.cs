using Modules.Settings.Contracts.Models;
using System.Windows;
using System.Windows.Controls;

namespace Modules.Settings.Views.Controls;

public class SettingsCategoryButton : Button
{
    public static readonly DependencyProperty IdProperty = DependencyProperty.Register(nameof(Id), typeof(SettingsPageType), typeof(SettingsCategoryButton));
    public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register(nameof(IsSelected), typeof(bool), typeof(SettingsCategoryButton));

    public SettingsPageType Id
    {
        get { return (SettingsPageType)GetValue(IdProperty); }
        set { SetValue(IdProperty, value); }
    }

    // TODO: make similar mechanism like activeCategory
    public bool IsSelected
    {
        get { return (bool)GetValue(IsSelectedProperty); }
        set { SetValue(IsSelectedProperty, value); }
    }
}
