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

    public void SetFocus()
    {
        TextBoxElement.Focus();
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
            var saveOnEnter = AppSettingsAccess.Current?.TaskPageSettings.SaveOnEnter == true;
            if (saveOnEnter || ctrlPressed && enter)
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
