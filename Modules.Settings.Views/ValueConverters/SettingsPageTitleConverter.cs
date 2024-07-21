using Modules.Common.Views.ValueConverters;
using System.Globalization;

namespace Modules.Settings.Views.ValueConverters;

public class SettingsPageTitleConverter : BaseValueConverter
{
    private readonly Dictionary<int, string> _pageTitles = new()
    {
        { 1, "Application settings" },
        { 2, "Themes" },
        { 3, "Page title settings" },
        { 4, "Task page settings" },
        { 5, "Task item settings" },
        { 6, "Tag settings" },
        { 7, "Task quick actions" },
        { 8, "Text editor quick actions" },
        { 9, "Note page settings" },
        { 10, "Date time settings" },
        { 11, "Shortcuts" },
    };

    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is int pageId)
        {
            if (_pageTitles.TryGetValue(pageId, out var title))
            {
                return title;
            }
        }

        return "TITLE CONVERTER ERROR";
    }
}
