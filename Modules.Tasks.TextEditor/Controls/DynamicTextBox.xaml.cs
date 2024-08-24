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
        DataContextChanged += OnDataContextChanged;
    }

    // When the listview virtualization mode is set to recycle,
    // the DataContext is changed to display a different item on the same list item.
    // Therefore, we handle the dataContext change to display the correct data on the item.
    private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        ContentControlElement.ContentTemplateSelector = new DynamicTextBoxSelector();
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
        var keyArrowUp = e.Key == Key.Up;
        var enter = e.Key == Key.Enter;
        var escape = e.Key == Key.Escape;
        var shiftPressed = Keyboard.Modifiers.HasFlag(ModifierKeys.Shift);
        var ctrlPressed = Keyboard.Modifiers.HasFlag(ModifierKeys.Control);
        var altPressed = Keyboard.Modifiers.HasFlag(ModifierKeys.Alt);

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
                        // TODO: clear focus
                    }
                }
            }
        }

        if (keyArrowUp && !shiftPressed && !ctrlPressed && !altPressed &&
            DataContext is DynamicTextBoxViewModel { IsEmpty: true } vm)
        {
            e.Handled = true;
            vm.OnQuickEditRequestedAction?.Invoke();
        }
    }
}
