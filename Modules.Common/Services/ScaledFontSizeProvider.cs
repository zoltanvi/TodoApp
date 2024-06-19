using Modules.Common.ViewModel;
using PropertyChanged;

namespace Modules.Common.Services;

[AddINotifyPropertyChangedInterface]
public class ScaledFontSizeProvider : BaseViewModel
{
    private readonly IUIScaler _uiScaler;

    private const double OriginalSmallest = 10;
    private const double OriginalSmaller = 12;
    private const double OriginalSmall = 14;
    private const double OriginalMedium = 16;
    private const double OriginalRegular = 18;
    private const double OriginalRegularIcon = 20;
    private const double OriginalLarge = 22;
    private const double OriginalLarger = 24;
    private const double OriginalHuge = 28;
    private const double OriginalLargeIcon = 30;
    private const double OriginalGiant = 40;

    public ScaledFontSizeProvider(IUIScaler uiScaler)
    {
        ArgumentNullException.ThrowIfNull(uiScaler);
        _uiScaler = uiScaler;
    }

    public double Smallest => OriginalSmallest * _uiScaler.ScaleValue;
    public double Smaller => OriginalSmaller * _uiScaler.ScaleValue;
    public double Small => OriginalSmall * _uiScaler.ScaleValue;
    public double Medium => OriginalMedium * _uiScaler.ScaleValue;
    public double Regular => OriginalRegular * _uiScaler.ScaleValue;
    public double Large => OriginalLarge * _uiScaler.ScaleValue;
    public double Larger => OriginalLarger * _uiScaler.ScaleValue;
    public double Huge => OriginalHuge * _uiScaler.ScaleValue;
    public double Giant => OriginalGiant * _uiScaler.ScaleValue;
    public double RegularIcon => OriginalRegularIcon * _uiScaler.ScaleValue;
    public double LargeIcon => OriginalLargeIcon * _uiScaler.ScaleValue;
}