using Modules.Common.Events;
using Modules.PopupMessage.Contracts;
using Prism.Events;

namespace Modules.Common.Services;

/// <summary>
/// App-wide UI scale. Scale changes broadcast via Prism <see cref="UiScaledViewEvent"/> (single pub/sub bus).
/// <see cref="IPopupMessageService"/> used for zoom percentage toast.
/// </summary>
public interface IUIScaler
{
    double ColorPickerHeight { get; }
    double ColorPickerItemSize { get; }
    double ColorPickerWidth { get; }
    double PortableColorPickerWidth { get; }
    double TaskPopupHeight { get; }
    ScaledFontSizeProvider FontSize { get; }
    double ScaleValue { get; }
    double SideMenuWidth { get; }
    double TextBoxMaxHeight { get; }
    double TextEditorToggleWidth { get; }
    double TaskCheckBoxWidth { get; }
    double SliderHeight { get; }
    double SliderThumbHeight { get; }
    double SliderThumbWidth { get; }
    double ScrollbarWidth { get; }
    double DbLocationTextBoxWidth { get; }
    double TaskProgressBarHeight { get; }

    void Setup(IPopupMessageService popupMessageService, IEventAggregator eventAggregator);
    void SetScaling(double value);
    void ZoomIn();
    void ZoomOut();
}