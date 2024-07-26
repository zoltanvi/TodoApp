using Modules.Common.Database;
using Modules.Common.DataBinding;
using Modules.Common.ViewModel;
using Modules.Settings.Contracts;
using PropertyChanged;
using System.IO;
using System.Text;
using System.Windows.Input;

namespace Modules.Settings.Views.Pages;

[AddINotifyPropertyChangedInterface]
public class ThemeSettingsPageViewModel : BaseViewModel
{
    private readonly IThemeEditorService _themeEditorService;

    public ThemeSettingsPageViewModel(IThemeEditorService themeEditorService)
    {
        ArgumentNullException.ThrowIfNull(themeEditorService);

        _themeEditorService = themeEditorService;

        DumpColorsCommand = new RelayCommand(DumpColors);
    }

    public string Primary
    {
        get => _themeEditorService.GetThemeColor(nameof(Primary));
        set => _themeEditorService.SetThemeColor(nameof(Primary), value);
    }

    public string OnPrimary
    {
        get => _themeEditorService.GetThemeColor(nameof(OnPrimary));
        set => _themeEditorService.SetThemeColor(nameof(OnPrimary), value);
    }

    public string PrimaryContainer
    {
        get => _themeEditorService.GetThemeColor(nameof(PrimaryContainer));
        set => _themeEditorService.SetThemeColor(nameof(PrimaryContainer), value);
    }

    public string OnPrimaryContainer
    {
        get => _themeEditorService.GetThemeColor(nameof(OnPrimaryContainer));
        set => _themeEditorService.SetThemeColor(nameof(OnPrimaryContainer), value);
    }

    public string Secondary
    {
        get => _themeEditorService.GetThemeColor(nameof(Secondary));
        set => _themeEditorService.SetThemeColor(nameof(Secondary), value);
    }

    public string OnSecondary
    {
        get => _themeEditorService.GetThemeColor(nameof(OnSecondary));
        set => _themeEditorService.SetThemeColor(nameof(OnSecondary), value);
    }

    public string SecondaryContainer
    {
        get => _themeEditorService.GetThemeColor(nameof(SecondaryContainer));
        set => _themeEditorService.SetThemeColor(nameof(SecondaryContainer), value);
    }

    public string OnSecondaryContainer
    {
        get => _themeEditorService.GetThemeColor(nameof(OnSecondaryContainer));
        set => _themeEditorService.SetThemeColor(nameof(OnSecondaryContainer), value);
    }

    public string Tertiary
    {
        get => _themeEditorService.GetThemeColor(nameof(Tertiary));
        set => _themeEditorService.SetThemeColor(nameof(Tertiary), value);
    }

    public string OnTertiary
    {
        get => _themeEditorService.GetThemeColor(nameof(OnTertiary));
        set => _themeEditorService.SetThemeColor(nameof(OnTertiary), value);
    }

    public string TertiaryContainer
    {
        get => _themeEditorService.GetThemeColor(nameof(TertiaryContainer));
        set => _themeEditorService.SetThemeColor(nameof(TertiaryContainer), value);
    }

    public string OnTertiaryContainer
    {
        get => _themeEditorService.GetThemeColor(nameof(OnTertiaryContainer));
        set => _themeEditorService.SetThemeColor(nameof(OnTertiaryContainer), value);
    }

    public string Error
    {
        get => _themeEditorService.GetThemeColor(nameof(Error));
        set => _themeEditorService.SetThemeColor(nameof(Error), value);
    }

    public string OnError
    {
        get => _themeEditorService.GetThemeColor(nameof(OnError));
        set => _themeEditorService.SetThemeColor(nameof(OnError), value);
    }

    public string ErrorContainer
    {
        get => _themeEditorService.GetThemeColor(nameof(ErrorContainer));
        set => _themeEditorService.SetThemeColor(nameof(ErrorContainer), value);
    }

    public string OnErrorContainer
    {
        get => _themeEditorService.GetThemeColor(nameof(OnErrorContainer));
        set => _themeEditorService.SetThemeColor(nameof(OnErrorContainer), value);
    }

    public string Background
    {
        get => _themeEditorService.GetThemeColor(nameof(Background));
        set => _themeEditorService.SetThemeColor(nameof(Background), value);
    }

    public string OnBackground
    {
        get => _themeEditorService.GetThemeColor(nameof(OnBackground));
        set => _themeEditorService.SetThemeColor(nameof(OnBackground), value);
    }

    public string Surface
    {
        get => _themeEditorService.GetThemeColor(nameof(Surface));
        set => _themeEditorService.SetThemeColor(nameof(Surface), value);
    }

    public string OnSurface
    {
        get => _themeEditorService.GetThemeColor(nameof(OnSurface));
        set => _themeEditorService.SetThemeColor(nameof(OnSurface), value);
    }

    public string SurfaceVariant
    {
        get => _themeEditorService.GetThemeColor(nameof(SurfaceVariant));
        set => _themeEditorService.SetThemeColor(nameof(SurfaceVariant), value);
    }

    public string OnSurfaceVariant
    {
        get => _themeEditorService.GetThemeColor(nameof(OnSurfaceVariant));
        set => _themeEditorService.SetThemeColor(nameof(OnSurfaceVariant), value);
    }

    public string Outline
    {
        get => _themeEditorService.GetThemeColor(nameof(Outline));
        set => _themeEditorService.SetThemeColor(nameof(Outline), value);
    }

    public string Shadow
    {
        get => _themeEditorService.GetThemeColor(nameof(Shadow));
        set => _themeEditorService.SetThemeColor(nameof(Shadow), value);
    }

    public string InverseSurface
    {
        get => _themeEditorService.GetThemeColor(nameof(InverseSurface));
        set => _themeEditorService.SetThemeColor(nameof(InverseSurface), value);
    }

    public string InverseOnSurface
    {
        get => _themeEditorService.GetThemeColor(nameof(InverseOnSurface));
        set => _themeEditorService.SetThemeColor(nameof(InverseOnSurface), value);
    }

    public string InversePrimary
    {
        get => _themeEditorService.GetThemeColor(nameof(InversePrimary));
        set => _themeEditorService.SetThemeColor(nameof(InversePrimary), value);
    }

    public string Surface1
    {
        get => _themeEditorService.GetThemeColor(nameof(Surface1));
        set => _themeEditorService.SetThemeColor(nameof(Surface1), value);
    }

    public string Surface2
    {
        get => _themeEditorService.GetThemeColor(nameof(Surface2));
        set => _themeEditorService.SetThemeColor(nameof(Surface2), value);
    }

    public string Surface3
    {
        get => _themeEditorService.GetThemeColor(nameof(Surface3));
        set => _themeEditorService.SetThemeColor(nameof(Surface3), value);
    }

    public string Surface4
    {
        get => _themeEditorService.GetThemeColor(nameof(Surface4));
        set => _themeEditorService.SetThemeColor(nameof(Surface4), value);
    }

    public string Surface5
    {
        get => _themeEditorService.GetThemeColor(nameof(Surface5));
        set => _themeEditorService.SetThemeColor(nameof(Surface5), value);
    }

    public string SurfaceDim
    {
        get => _themeEditorService.GetThemeColor(nameof(SurfaceDim));
        set => _themeEditorService.SetThemeColor(nameof(SurfaceDim), value);
    }

    public string SurfaceBright
    {
        get => _themeEditorService.GetThemeColor(nameof(SurfaceBright));
        set => _themeEditorService.SetThemeColor(nameof(SurfaceBright), value);
    }

    public string SurfaceContainerLowest
    {
        get => _themeEditorService.GetThemeColor(nameof(SurfaceContainerLowest));
        set => _themeEditorService.SetThemeColor(nameof(SurfaceContainerLowest), value);
    }

    public string SurfaceContainerLow
    {
        get => _themeEditorService.GetThemeColor(nameof(SurfaceContainerLow));
        set => _themeEditorService.SetThemeColor(nameof(SurfaceContainerLow), value);
    }

    public string SurfaceContainer
    {
        get => _themeEditorService.GetThemeColor(nameof(SurfaceContainer));
        set => _themeEditorService.SetThemeColor(nameof(SurfaceContainer), value);
    }

    public string SurfaceContainerHigh
    {
        get => _themeEditorService.GetThemeColor(nameof(SurfaceContainerHigh));
        set => _themeEditorService.SetThemeColor(nameof(SurfaceContainerHigh), value);
    }

    public string SurfaceContainerHighest
    {
        get => _themeEditorService.GetThemeColor(nameof(SurfaceContainerHighest));
        set => _themeEditorService.SetThemeColor(nameof(SurfaceContainerHighest), value);
    }

    public string OutlineVariant
    {
        get => _themeEditorService.GetThemeColor(nameof(OutlineVariant));
        set => _themeEditorService.SetThemeColor(nameof(OutlineVariant), value);
    }

    public ICommand DumpColorsCommand { get; }

    private void DumpColors()
    {
        var dbFolder = Path.GetDirectoryName(DbConfiguration.DatabasePath);
        var themeFolder = Path.Combine(dbFolder, "SavedThemes");
        Directory.CreateDirectory(themeFolder);

        var fileName = $"CustomTheme_{DateTime.Now:yyyy-MM-dd__HH_mm_ss_ffff}.txt";
        var themeFilePath = Path.Combine(themeFolder, fileName);

        StringBuilder sb = new StringBuilder();

        sb.AppendLine($"{nameof(Primary)} = {Primary}");
        sb.AppendLine($"{nameof(OnPrimary)} = {OnPrimary}");
        sb.AppendLine($"{nameof(PrimaryContainer)} = {PrimaryContainer}");
        sb.AppendLine($"{nameof(OnPrimaryContainer)} = {OnPrimaryContainer}");
        sb.AppendLine($"{nameof(Secondary)} = {Secondary}");
        sb.AppendLine($"{nameof(OnSecondary)} = {OnSecondary}");
        sb.AppendLine($"{nameof(SecondaryContainer)} = {SecondaryContainer}");
        sb.AppendLine($"{nameof(OnSecondaryContainer)} = {OnSecondaryContainer}");
        sb.AppendLine($"{nameof(Tertiary)} = {Tertiary}");
        sb.AppendLine($"{nameof(OnTertiary)} = {OnTertiary}");
        sb.AppendLine($"{nameof(TertiaryContainer)} = {TertiaryContainer}");
        sb.AppendLine($"{nameof(OnTertiaryContainer)} = {OnTertiaryContainer}");
        sb.AppendLine($"{nameof(Error)} = {Error}");
        sb.AppendLine($"{nameof(OnError)} = {OnError}");
        sb.AppendLine($"{nameof(ErrorContainer)} = {ErrorContainer}");
        sb.AppendLine($"{nameof(OnErrorContainer)} = {OnErrorContainer}");
        sb.AppendLine($"{nameof(Background)} = {Background}");
        sb.AppendLine($"{nameof(OnBackground)} = {OnBackground}");
        sb.AppendLine($"{nameof(Surface)} = {Surface}");
        sb.AppendLine($"{nameof(OnSurface)} = {OnSurface}");
        sb.AppendLine($"{nameof(SurfaceVariant)} = {SurfaceVariant}");
        sb.AppendLine($"{nameof(OnSurfaceVariant)} = {OnSurfaceVariant}");
        sb.AppendLine($"{nameof(Outline)} = {Outline}");
        sb.AppendLine($"{nameof(Shadow)} = {Shadow}");
        sb.AppendLine($"{nameof(InverseSurface)} = {InverseSurface}");
        sb.AppendLine($"{nameof(InverseOnSurface)} = {InverseOnSurface}");
        sb.AppendLine($"{nameof(InversePrimary)} = {InversePrimary}");
        sb.AppendLine($"{nameof(Surface1)} = {Surface1}");
        sb.AppendLine($"{nameof(Surface2)} = {Surface2}");
        sb.AppendLine($"{nameof(Surface3)} = {Surface3}");
        sb.AppendLine($"{nameof(Surface4)} = {Surface4}");
        sb.AppendLine($"{nameof(Surface5)} = {Surface5}");
        sb.AppendLine($"{nameof(SurfaceDim)} = {SurfaceDim}");
        sb.AppendLine($"{nameof(SurfaceBright)} = {SurfaceBright}");
        sb.AppendLine($"{nameof(SurfaceContainerLowest)} = {SurfaceContainerLowest}");
        sb.AppendLine($"{nameof(SurfaceContainerLow)} = {SurfaceContainerLow}");
        sb.AppendLine($"{nameof(SurfaceContainer)} = {SurfaceContainer}");
        sb.AppendLine($"{nameof(SurfaceContainerHigh)} = {SurfaceContainerHigh}");
        sb.AppendLine($"{nameof(SurfaceContainerHighest)} = {SurfaceContainerHighest}");
        sb.AppendLine($"{nameof(OutlineVariant)} = {OutlineVariant}");


        File.AppendAllText(themeFilePath, sb.ToString());
    }
}
