namespace Modules.Settings.Contracts.ViewModels;

public sealed class AppSettings : SettingsBase
{
    public static AppSettings Instance { get; } = new();

    public AppWindowSettings AppWindowSettings { get; set; } = new();
    public ThemeSettings ThemeSettings { get; set; } = new();
    public PageTitleSettings PageTitleSettings { get; set; } = new();
    public TaskPageSettings TaskPageSettings { get; set; } = new();
    public TaskSettings TaskSettings { get; set; } = new();
    public TaskQuickActionSettings TaskQuickActionSettings { get; set; } = new();
    public TextEditorQuickActionSettings TextEditorQuickActionSettings { get; set; } = new();
    public NoteSettings NoteSettings { get; set; } = new();
    public WindowSettings WindowSettings { get; set; } = new();
    public DateTimeSettings DateTimeSettings { get; set; } = new();
    public SessionSettings SessionSettings { get; set; } = new();

    private AppSettings() { }
}
