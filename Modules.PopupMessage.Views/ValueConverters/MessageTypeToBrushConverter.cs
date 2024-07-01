using Modules.Common.DataModels;
using Modules.Common.Views.ValueConverters;
using Modules.PopupMessage.Views.Models;
using System.Globalization;

namespace Modules.PopupMessage.Views.ValueConverters;

internal class MessageTypeToBrushConverter : BaseValueConverter
{
    private readonly TagColorConverter _tagColorConverter = new();
    public ColorType ColorType { get; set; }

    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is MessageType messageType)
        {
            _tagColorConverter.ColorType = ColorType;
            switch (messageType)
            {
                case MessageType.Warning:
                    return _tagColorConverter.Convert(TagPresetColor.Lime);
                case MessageType.Info:
                    return _tagColorConverter.Convert(TagPresetColor.Geekblue);
                case MessageType.Error:
                    return _tagColorConverter.Convert(TagPresetColor.Magenta);
                case MessageType.Success:
                    return _tagColorConverter.Convert(TagPresetColor.Green);
            }
        }

        return null;
    }
}