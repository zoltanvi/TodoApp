using System.Windows;

namespace TodoApp;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
/// 
public partial class App : Application
{
    protected override void OnActivated(EventArgs e)
    {
        base.OnActivated(e);

        var enumToFontFamilyConverter = new Modules.Common.Views.ValueConverters.EnumToFontFamilyConverter();
        var version = Modules.Common.Constants.CurrentVersion;
    }
}

