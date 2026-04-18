namespace Modules.Settings.Contracts.ViewModels;

/// <summary>
/// Application-wide settings aggregate. Resolved as a singleton (same instance as <see cref="AppSettings"/>).
/// </summary>
public interface IAppSettings
{
    ApplicationSettings ApplicationSettings { get; set; }
    ThemeSettings ThemeSettings { get; set; }
    PageTitleSettings PageTitleSettings { get; set; }
    TaskPageSettings TaskPageSettings { get; set; }
    TaskSettings TaskSettings { get; set; }
    TaskQuickActionSettings TaskQuickActionSettings { get; set; }
    NoteSettings NoteSettings { get; set; }
    WindowSettings WindowSettings { get; set; }
    DateTimeSettings DateTimeSettings { get; set; }
    SessionSettings SessionSettings { get; set; }
    CategorySettings CategorySettings { get; set; }

    bool IsDirty();
    void Clean();
}
