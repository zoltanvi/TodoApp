using Modules.Categories.Views.Pages;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Modules.Categories.Views.Controls;

public static class RenameBehavior
{
    public static readonly DependencyProperty IsInRenameModeProperty =
        DependencyProperty.RegisterAttached(
            "IsInRenameMode",
            typeof(bool),
            typeof(RenameBehavior),
            new PropertyMetadata(false, OnIsInRenameModeChanged));

    public static bool GetIsInRenameMode(DependencyObject obj) =>
        (bool)obj.GetValue(IsInRenameModeProperty);

    public static void SetIsInRenameMode(DependencyObject obj, bool value) =>
        obj.SetValue(IsInRenameModeProperty, value);

    private static void OnIsInRenameModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not TextBox textBox) return;

        if (e.NewValue is true)
        {
            textBox.Focus();
            textBox.SelectAll();
            textBox.KeyDown += OnKeyDown;
            textBox.LostFocus += OnLostFocus;
        }
        else
        {
            textBox.KeyDown -= OnKeyDown;
            textBox.LostFocus -= OnLostFocus;
        }
    }

    private static void OnKeyDown(object sender, KeyEventArgs e)
    {
        if (sender is not TextBox textBox) return;
        if (textBox.DataContext is not CategoryItemViewModel vm) return;

        if (e.Key == Key.Enter)
        {
            var pageVm = FindCategoryPageViewModel(textBox);
            pageVm?.FinishRename(vm.Id, textBox.Text);
            e.Handled = true;
        }
        else if (e.Key == Key.Escape)
        {
            var pageVm = FindCategoryPageViewModel(textBox);
            pageVm?.CancelRename(vm.Id);
            e.Handled = true;
        }
    }

    private static void OnLostFocus(object sender, RoutedEventArgs e)
    {
        if (sender is not TextBox textBox) return;
        if (textBox.DataContext is not CategoryItemViewModel vm) return;
        if (!vm.IsRenaming) return;

        var pageVm = FindCategoryPageViewModel(textBox);
        pageVm?.FinishRename(vm.Id, textBox.Text);
    }

    private static CategoryPageViewModel? FindCategoryPageViewModel(FrameworkElement element)
    {
        var parent = element;
        while (parent != null)
        {
            if (parent is FrameworkElement fe && fe.Tag is CategoryPageViewModel pageVm)
            {
                return pageVm;
            }

            if (parent.DataContext is CategoryPageViewModel vm)
            {
                return vm;
            }

            parent = parent.Parent as FrameworkElement;
        }
        return null;
    }
}
