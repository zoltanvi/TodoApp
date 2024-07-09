namespace Modules.Common.OBSOLETE.Mediator;

/// <summary>
/// Defines the available message types for the mediator and it's clients
/// </summary>
public enum ViewModelMessages
{
    /// <summary>
    /// The Application theme changed
    /// </summary>
    ThemeChanged,

    /// <summary>
    /// Ctrl + Shift + L has been pressed in a RichTextBox
    /// </summary>
    NextThemeWithHotkeyRequested,

    /// <summary>
    /// Task page's bottom text editor should be focused
    /// </summary>
    FocusBottomTextEditor
}