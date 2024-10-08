﻿using MediatR;
using Modules.Common.Cqrs.Events;
using Modules.Common.Events;
using Modules.Common.Services;
using Modules.Common.ViewModel;
using Modules.PopupMessage.Contracts.Cqrs.Commands;
using Prism.Events;
using PropertyChanged;

namespace Modules.Common.Views.Services;

[AddINotifyPropertyChangedInterface]
public class UIScaler : BaseViewModel, IUIScaler
{
    private const double OriginalScalingPercent = 100;
    private const double OriginalSideMenuWidth = 220;
    private const double OriginalTextBoxMaxHeight = 400;
    private const double OriginalDbLocationTextBoxWidth = 300;
    private const double OriginalPortableColorPickerWidth = 26;
    private const double OriginalColorPickerHeight = 31;
    private const double OriginalColorPickerWidth = 56;
    private const double OriginalColorPickerItemSize = 21;
    private const double OriginalTaskPopupHeight = 31;
    private const double OriginalTextEditorToggleWidth = 15;
    private const double OriginalTaskProgressBarHeight = 5;
    private const int ColorPickerColumns = 9;

    private double _scalingPercent = OriginalScalingPercent;
    private IMediator? _mediator;
    private IEventAggregator? _eventAggregator;

    public static IUIScaler Instance { get; } = new UIScaler();

    private UIScaler()
    {
        FontSize = new ScaledFontSizeProvider(this);
    }

    public static double StaticScaleValue { get; private set; } = 1;
    public double ScaleValue => StaticScaleValue;
    public ScaledFontSizeProvider FontSize { get; }
    public double SideMenuWidth => OriginalSideMenuWidth * ScaleValue;
    public double DbLocationTextBoxWidth => OriginalDbLocationTextBoxWidth * ScaleValue;
    public double TextBoxMaxHeight => OriginalTextBoxMaxHeight * ScaleValue;
    public double ColorPickerHeight => OriginalColorPickerHeight * ScaleValue;
    public double ColorPickerWidth => OriginalColorPickerWidth * ScaleValue;
    public double ColorPickerItemSize => OriginalColorPickerItemSize * ScaleValue;
    public double TaskPopupHeight => OriginalTaskPopupHeight * ScaleValue;
    public double PortableColorPickerWidth => OriginalPortableColorPickerWidth * ScaleValue;
    public double TextEditorToggleWidth => OriginalTextEditorToggleWidth * ScaleValue;
    public double TaskCheckBoxWidth => 8 * ScaleValue;
    public double SliderHeight => 18 * ScaleValue;
    public double SliderThumbHeight => 18 * ScaleValue;
    public double SliderThumbWidth => 20;
    public double ScrollbarWidth => 6 * ScaleValue;
    public double TaskProgressBarHeight => OriginalTaskProgressBarHeight * ScaleValue;

    public void Setup(IMediator mediator, IEventAggregator eventAggregator)
    {
        ArgumentNullException.ThrowIfNull(mediator);
        ArgumentNullException.ThrowIfNull(eventAggregator);

        _mediator = mediator;
        _eventAggregator = eventAggregator;
    }

    public void ZoomOut()
    {
        Zoom(false);
    }

    public void ZoomIn()
    {
        Zoom(true);
    }

    public void SetScaling(double value)
    {
        value = Math.Round(value, 3);
        var zoomed = Math.Abs(value - StaticScaleValue) > 0.00001;
        var oldScaleValue = StaticScaleValue;

        StaticScaleValue = value;
        _scalingPercent = Math.Round(StaticScaleValue * OriginalScalingPercent, 3);

        OnPropertyChanged(string.Empty);

        if (zoomed)
        {
            _mediator?.Publish(new UiScaledEvent
            {
                OldScaleValue = oldScaleValue,
                NewScaleValue = StaticScaleValue
            });

            _eventAggregator?.GetEvent<UiScaledViewEvent>().Publish(new UiScaledViewPayload 
            {
                OldScaleValue = oldScaleValue,
                NewScaleValue = StaticScaleValue
            });
        }
    }

    private void Zoom(bool zoomIn)
    {
        double zoomOffset = 0;
        const double maxScalingPercent = 500;
        const double minScalingPercent = 30;

        if (zoomIn && _scalingPercent < maxScalingPercent)
        {
            // Zoom in
            zoomOffset = 10;
        }
        else if (!zoomIn && _scalingPercent > minScalingPercent)
        {
            // Zoom out
            zoomOffset = -10;
        }

        _scalingPercent += zoomOffset;
        SetScaling(_scalingPercent / OriginalScalingPercent);

        _mediator?.Send(new ShowMessageInfoCommand
        {
            Message = $"{_scalingPercent} %"
        });
    }
}
