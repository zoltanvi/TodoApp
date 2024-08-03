using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Modules.Common.Views.Controls;

public class SubmitEscapeTextBox : TextBox
{
    public static readonly DependencyProperty EscapePressedCommandProperty = DependencyProperty.Register(nameof(EscapePressedCommand), typeof(ICommand), typeof(SubmitEscapeTextBox));
    public static readonly DependencyProperty EnterPressedCommandProperty = DependencyProperty.Register(nameof(EnterPressedCommand), typeof(ICommand), typeof(SubmitEscapeTextBox));

    public ICommand EscapePressedCommand
    {
        get => (ICommand)GetValue(EscapePressedCommandProperty);
        set => SetValue(EscapePressedCommandProperty, value);
    }

    public ICommand EnterPressedCommand
    {
        get => (ICommand)GetValue(EnterPressedCommandProperty);
        set => SetValue(EnterPressedCommandProperty, value);
    }

    protected override void OnPreviewKeyDown(KeyEventArgs e)
    {
        base.OnPreviewKeyDown(e);

        var escape = e.Key == Key.Escape;
        var enter = e.Key == Key.Enter;

        if (escape)
        {
            EscapePressedCommand?.Execute(null);
        }

        if (enter)
        {
            EnterPressedCommand?.Execute(null);
        }
    }
}