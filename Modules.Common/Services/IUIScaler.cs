﻿namespace Modules.Common.Services;

public interface IUIScaler
{
    double ColorPickerDropDownWidth { get; }
    double ColorPickerHeight { get; }
    double ColorPickerItemSize { get; }
    double ColorPickerWidth { get; }
    ScaledFontSizeProvider FontSize { get; }
    double ScaleValue { get; }
    double SideMenuWidth { get; }
    double TextBoxMaxHeight { get; }
    double TextEditorToggleWidth { get; }
    double TaskCheckBoxWidth { get; }
    double SliderHeight { get; }
    double SliderThumbHeight { get; }
    double SliderThumbWidth { get; }

    void SetScaling(double value);
    void ZoomIn();
    void ZoomOut();
}