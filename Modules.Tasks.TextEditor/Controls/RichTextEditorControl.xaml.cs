using Modules.Tasks.TextEditor.Services;
using System.Windows;
using System.Windows.Controls;

namespace Modules.Tasks.TextEditor.Controls;

public partial class RichTextEditorControl : UserControl
{
    private SingletonToolbar? _toolbar;

    public static readonly DependencyProperty TextOpacityProperty = 
        DependencyProperty.Register(nameof(TextOpacity), typeof(double), typeof(RichTextEditorControl), new PropertyMetadata());

    public static readonly DependencyProperty IsReadOnlyProperty = 
        DependencyProperty.Register(nameof(IsReadOnly), typeof(bool), typeof(RichTextEditorControl), new PropertyMetadata());

    public double TextOpacity
    {
        get { return (double)GetValue(TextOpacityProperty); }
        set { SetValue(TextOpacityProperty, value); }
    }

    public bool IsReadOnly
    {
        get { return (bool)GetValue(IsReadOnlyProperty); }
        set { SetValue(IsReadOnlyProperty, value); }
    }

    public RichTextEditorControl()
    {
        InitializeComponent();
        IsEditorOpenToggle.Checked += IsEditorOpenToggle_Checked;
        IsEditorOpenToggle.Unchecked += IsEditorOpenToggle_Unchecked;
        PartTextEditor.CtrlShiftEnterAction = ToggleEditorOpen;
    }

    public void SetFocus()
    {
        PartTextEditor.Focus();
    }

    private void IsEditorOpenToggle_Checked(object sender, RoutedEventArgs e)
    {
        if (ToolBarPanel.Children.Count == 0)
        {
            _toolbar ??= ToolbarService.Toolbar;

            _toolbar.SetParentStackPanel(ToolBarPanel);

            SetToolbarActions(_toolbar);

            PartTextEditor.StatePropertyChanged += OnTextEditorStatePropertyChanged;
        }
    }

    private void ToggleEditorOpen()
    {
        IsEditorOpenToggle.IsChecked = !IsEditorOpenToggle.IsChecked;
    }

    private void IsEditorOpenToggle_Unchecked(object sender, RoutedEventArgs e)
    {
        if (ToolBarPanel.Children.Count != 0)
        {
            ToolBarPanel.Children.Clear();
            PartTextEditor.StatePropertyChanged -= OnTextEditorStatePropertyChanged;
        }
    }

    private void SetToolbarActions(SingletonToolbar toolbar)
    {
        toolbar.SetBoldCommandAction(() => PartTextEditor.SetBoldCommand?.Execute(null));
        toolbar.SetItalicCommandAction(() => PartTextEditor.SetItalicCommand?.Execute(null));
        toolbar.SetUnderlinedCommandAction(() => PartTextEditor.SetUnderlinedCommand?.Execute(null));
        toolbar.SetSmallFontSizeCommandAction(() => PartTextEditor.SetSmallFontSizeCommand?.Execute(null));
        toolbar.SetMediumFontSizeCommandAction(() => PartTextEditor.SetMediumFontSizeCommand?.Execute(null));
        toolbar.SetBigFontSizeCommandAction(() => PartTextEditor.SetBigFontSizeCommand?.Execute(null));
        toolbar.IncreaseFontSizeCommandAction(() => PartTextEditor.IncreaseFontSizeCommand?.Execute(null));
        toolbar.DecreaseFontSizeCommandAction(() => PartTextEditor.DecreaseFontSizeCommand?.Execute(null));
        toolbar.ResetFormattingCommandAction(() => PartTextEditor.ResetFormattingCommand?.Execute(null));
        toolbar.MonospaceCommandAction(() => PartTextEditor.MonospaceCommand?.Execute(null));
        toolbar.AlignLeftCommandAction(() => PartTextEditor.AlignLeftCommand?.Execute(null));
        toolbar.AlignCenterCommandAction(() => PartTextEditor.AlignCenterCommand?.Execute(null));
        toolbar.AlignRightCommandAction(() => PartTextEditor.AlignRightCommand?.Execute(null));
        toolbar.AlignJustifyCommandAction(() => PartTextEditor.AlignJustifyCommand?.Execute(null));
    }

    private void OnTextEditorStatePropertyChanged(object sender, EventArgs e)
    {
        ArgumentNullException.ThrowIfNull(_toolbar);
        _toolbar.IsSelectionBold = PartTextEditor.IsSelectionBold;
        _toolbar.IsSelectionItalic = PartTextEditor.IsSelectionItalic;
        _toolbar.IsSelectionUnderlined = PartTextEditor.IsSelectionUnderlined;
    }
}
