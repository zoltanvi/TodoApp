using Modules.Settings.Contracts.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Modules.Tasks.TextEditor.Controls;

public partial class DynamicTextBox : UserControl
{
    public DynamicTextBox()
    {
        InitializeComponent();
    }

    public static readonly DependencyProperty IsPlainTextModeProperty = DependencyProperty.Register(nameof(IsPlainTextMode), typeof(bool), typeof(DynamicTextBox), new PropertyMetadata(true, OnIsPlainTextModeChanged));
    public static readonly DependencyProperty IsReadOnlyProperty = DependencyProperty.Register(nameof(IsReadOnly), typeof(bool), typeof(DynamicTextBox), new PropertyMetadata(default(bool)));

    public bool IsPlainTextMode
    {
        get => (bool)GetValue(IsPlainTextModeProperty);
        set => SetValue(IsPlainTextModeProperty, value);
    }

    public bool IsReadOnly
    {
        get => (bool)GetValue(IsReadOnlyProperty);
        set => SetValue(IsReadOnlyProperty, value);
    }

    private static void OnIsPlainTextModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (DynamicTextBox)d;
        control.ContentControlElement.ContentTemplateSelector = new DynamicTextBoxSelector();
    }

    public void SetFocus()
    {
        // TODO:
    }

    private void TextBoxElement_OnPreviewKeyDown(object sender, KeyEventArgs e)
    {
        var enter = e.Key == Key.Enter;
        var escape = e.Key == Key.Escape;
        var shiftPressed = Keyboard.Modifiers.HasFlag(ModifierKeys.Shift);
        var ctrlPressed = Keyboard.Modifiers.HasFlag(ModifierKeys.Control);

        if (escape || enter && !shiftPressed)
        {
            if (AppSettings.Instance.TaskPageSettings.SaveOnEnter || ctrlPressed && enter)
            {
                if (DataContext is DynamicTextBoxViewModel viewModel)
                {
                    var wasEmpty = viewModel.IsEmpty;
                    viewModel.EnterAction?.Invoke();
                    e.Handled = true;

                    if (escape && wasEmpty)
                    {
                        Keyboard.ClearFocus();
                    }
                }
            }
        }
    }
}
