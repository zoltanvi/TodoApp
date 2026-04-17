using System.Globalization;
using System.Windows.Data;

namespace Modules.Categories.Views.Controls;

public class BoolToFolderIconConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is true)
        {
            return "\uE8B7"; // Folder icon
        }

        return "\uE974"; // List icon
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
