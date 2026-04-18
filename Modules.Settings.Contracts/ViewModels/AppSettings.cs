namespace Modules.Settings.Contracts.ViewModels;

public sealed class AppSettings : SettingsBase
{
    public static AppSettings Instance { get; } = new();

    public ApplicationSettings ApplicationSettings { get; set; } = new();
    public ThemeSettings ThemeSettings { get; set; } = new();
    public PageTitleSettings PageTitleSettings { get; set; } = new();
    public TaskPageSettings TaskPageSettings { get; set; } = new();
    public TaskSettings TaskSettings { get; set; } = new();
    public TaskQuickActionSettings TaskQuickActionSettings { get; set; } = new();
    public NoteSettings NoteSettings { get; set; } = new();
    public WindowSettings WindowSettings { get; set; } = new();
    public DateTimeSettings DateTimeSettings { get; set; } = new();
    public SessionSettings SessionSettings { get; set; } = new();
    public CategorySettings CategorySettings { get; set; } = new();

    public override bool IsDirty() =>
        ApplicationSettings.IsDirty() ||
        ThemeSettings.IsDirty() ||
        PageTitleSettings.IsDirty() ||
        TaskPageSettings.IsDirty() ||
        TaskSettings.IsDirty() ||
        TaskQuickActionSettings.IsDirty() ||
        NoteSettings.IsDirty() ||
        WindowSettings.IsDirty() ||
        DateTimeSettings.IsDirty() ||
        SessionSettings.IsDirty() ||
        CategorySettings.IsDirty();

    public override void Clean()
    {
        ApplicationSettings.Clean();
        ThemeSettings.Clean();
        PageTitleSettings.Clean();
        TaskPageSettings.Clean();
        TaskSettings.Clean();
        TaskQuickActionSettings.Clean();
        NoteSettings.Clean();
        WindowSettings.Clean();
        DateTimeSettings.Clean();
        SessionSettings.Clean();
        CategorySettings.Clean();
    }

    private AppSettings() { }
}
