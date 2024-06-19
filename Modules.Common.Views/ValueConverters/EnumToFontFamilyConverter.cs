using Modules.Common.DataModels;
using System.Globalization;
using System.Windows;
using MediaFontFamily = System.Windows.Media.FontFamily;

namespace Modules.Common.Views.ValueConverters;

/// <summary>
/// A converter that takes in a <see cref="FontFamily"/> and converts it to a <see cref="MediaFontFamily"/>
/// </summary>
public class EnumToFontFamilyConverter : BaseValueConverter
{
    private static bool _initialized;
    private static readonly MediaFontFamily DefaultFontFamily = new(Constants.FontFamily.SegoeUI);
    private static readonly Dictionary<FontFamily, MediaFontFamily> FontFamilies = [];

    public static EnumToFontFamilyConverter Instance { get; } = new();

    public EnumToFontFamilyConverter()
    {
        if (!_initialized)
        {
            _initialized = true;
            AddDefaultFontFamily(FontFamily.Calibri, Constants.FontFamily.Calibri);
            AddDefaultFontFamily(FontFamily.Consolas, Constants.FontFamily.Consolas);
            AddDefaultFontFamily(FontFamily.CourierNew, Constants.FontFamily.CourierNew);

            AddFontFamily(FontFamily.FiraSansLight);
            AddFontFamily(FontFamily.FiraSansRegular);

            AddFontFamily(FontFamily.InterLight);
            AddFontFamily(FontFamily.InterRegular);

            AddFontFamily(FontFamily.MontserratAlternatesLight);
            AddFontFamily(FontFamily.MontserratAlternatesRegular);

            AddFontFamily(FontFamily.MontserratLight);
            AddFontFamily(FontFamily.MontserratRegular);

            AddFontFamily(FontFamily.NotoSansLight);
            AddFontFamily(FontFamily.NotoSansRegular);

            AddFontFamily(FontFamily.OpenSansLight);
            AddFontFamily(FontFamily.OpenSans);


            AddDefaultFontFamily(FontFamily.SegoeUILight, Constants.FontFamily.SegoeUILight);
            AddDefaultFontFamily(FontFamily.SegoeUI, Constants.FontFamily.SegoeUI);
            AddDefaultFontFamily(FontFamily.SegoeUIBold, Constants.FontFamily.SegoeUIBold);

            AddDefaultFontFamily(FontFamily.TimesNewRoman, Constants.FontFamily.TimesNewRoman);

            AddFontFamily(FontFamily.UbuntuLight);
            AddFontFamily(FontFamily.UbuntuRegular);

            AddDefaultFontFamily(FontFamily.Verdana, Constants.FontFamily.Verdana);
        }
    }

    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var fValue = (FontFamily)value;
        return FontFamilies.GetValueOrDefault(fValue, DefaultFontFamily);
    }

    public MediaFontFamily Convert(object value)
    {
        return (MediaFontFamily)Convert(value, null, null, null);
    }

    private static void AddDefaultFontFamily(FontFamily font, string familyName)
    {
        FontFamilies.Add(font, new MediaFontFamily(familyName));
    }

    private void AddFontFamily(FontFamily font)
    {
        var fontFamily = (MediaFontFamily)Application.Current.TryFindResource(Enum.GetName(typeof(FontFamily), font));
        FontFamilies.Add(font, fontFamily);
    }
}