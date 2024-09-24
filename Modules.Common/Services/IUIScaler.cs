using MediatR;
using Prism.Events;

namespace Modules.Common.Services;

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

    void Setup(IMediator mediator, IEventAggregator eventAggregator);
    void SetScaling(double value);
    void ZoomIn();
    void ZoomOut();
}