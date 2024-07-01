using Modules.Common.DataBinding;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Modules.Tasks.TextEditor.Controls;
public class SingletonToolbar : Label
{
    public static readonly DependencyProperty IsSelectionBoldProperty = DependencyProperty.Register(nameof(IsSelectionBold), typeof(bool), typeof(SingletonToolbar), new PropertyMetadata());
    public static readonly DependencyProperty IsSelectionItalicProperty = DependencyProperty.Register(nameof(IsSelectionItalic), typeof(bool), typeof(SingletonToolbar), new PropertyMetadata());
    public static readonly DependencyProperty IsSelectionUnderlinedProperty = DependencyProperty.Register(nameof(IsSelectionUnderlined), typeof(bool), typeof(SingletonToolbar), new PropertyMetadata());

    public StackPanel? ParentStackPanel { get; private set; }

    public bool IsSelectionBold
    {
        get { return (bool)GetValue(IsSelectionBoldProperty); }
        set { SetValue(IsSelectionBoldProperty, value); }
    }

    public bool IsSelectionItalic
    {
        get { return (bool)GetValue(IsSelectionItalicProperty); }
        set { SetValue(IsSelectionItalicProperty, value); }
    }
    public bool IsSelectionUnderlined
    {
        get { return (bool)GetValue(IsSelectionUnderlinedProperty); }
        set { SetValue(IsSelectionUnderlinedProperty, value); }
    }

    public ICommand SetBoldCommand { get; } = new LazyRelayCommand();
    public ICommand SetItalicCommand { get; } = new LazyRelayCommand();
    public ICommand SetUnderlinedCommand { get; } = new LazyRelayCommand();
    public ICommand SetSmallFontSizeCommand { get; } = new LazyRelayCommand();
    public ICommand SetMediumFontSizeCommand { get; } = new LazyRelayCommand();
    public ICommand SetBigFontSizeCommand { get; } = new LazyRelayCommand();
    public ICommand IncreaseFontSizeCommand { get; } = new LazyRelayCommand();
    public ICommand DecreaseFontSizeCommand { get; } = new LazyRelayCommand();
    public ICommand ResetFormattingCommand { get; } = new LazyRelayCommand();
    public ICommand MonospaceCommand { get; } = new LazyRelayCommand();
    public ICommand AlignLeftCommand { get; } = new LazyRelayCommand();
    public ICommand AlignCenterCommand { get; } = new LazyRelayCommand();
    public ICommand AlignRightCommand { get; } = new LazyRelayCommand();
    public ICommand AlignJustifyCommand { get; } = new LazyRelayCommand();

    public void SetParentStackPanel(StackPanel stackPanel)
    {
        // Remove from previous parent
        ParentStackPanel?.Children.Clear();

        ParentStackPanel = stackPanel;
        ParentStackPanel.Children.Add(this);
    }

    public void SetBoldCommandAction(Action action) => ((LazyRelayCommand)SetBoldCommand).SetAction(action);
    public void SetItalicCommandAction(Action action) => ((LazyRelayCommand)SetItalicCommand).SetAction(action);
    public void SetUnderlinedCommandAction(Action action) => ((LazyRelayCommand)SetUnderlinedCommand).SetAction(action);
    public void SetSmallFontSizeCommandAction(Action action) => ((LazyRelayCommand)SetSmallFontSizeCommand).SetAction(action);
    public void SetMediumFontSizeCommandAction(Action action) => ((LazyRelayCommand)SetMediumFontSizeCommand).SetAction(action);
    public void SetBigFontSizeCommandAction(Action action) => ((LazyRelayCommand)SetBigFontSizeCommand).SetAction(action);
    public void IncreaseFontSizeCommandAction(Action action) => ((LazyRelayCommand)IncreaseFontSizeCommand).SetAction(action);
    public void DecreaseFontSizeCommandAction(Action action) => ((LazyRelayCommand)DecreaseFontSizeCommand).SetAction(action);
    public void ResetFormattingCommandAction(Action action) => ((LazyRelayCommand)ResetFormattingCommand).SetAction(action);
    public void MonospaceCommandAction(Action action) => ((LazyRelayCommand)MonospaceCommand).SetAction(action);
    public void AlignLeftCommandAction(Action action) => ((LazyRelayCommand)AlignLeftCommand).SetAction(action);
    public void AlignCenterCommandAction(Action action) => ((LazyRelayCommand)AlignCenterCommand).SetAction(action);
    public void AlignRightCommandAction(Action action) => ((LazyRelayCommand)AlignRightCommand).SetAction(action);
    public void AlignJustifyCommandAction(Action action) => ((LazyRelayCommand)AlignJustifyCommand).SetAction(action);
}
