namespace Modules.Settings.Contracts;

public interface IThemeEditorService
{
    void SetThemeColor(string resourceName, string value);
    string GetThemeColor(string resourceName);
}
