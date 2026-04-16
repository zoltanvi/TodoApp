using System.Windows;

namespace Modules.Tasks.Views.AttachedProperties;

public static class TaskPopupMarginScaler
{
    public static readonly DependencyProperty VerticalOffsetProperty = DependencyProperty.RegisterAttached("VerticalOffset", typeof(double), typeof(TaskPopupMarginScaler), new PropertyMetadata(0.0, OnPropertyChanged));
    public static readonly DependencyProperty HorizontalOffsetProperty = DependencyProperty.RegisterAttached("HorizontalOffset", typeof(double), typeof(TaskPopupMarginScaler), new PropertyMetadata(0.0, OnPropertyChanged));
    public static readonly DependencyProperty ScaleValueProperty = DependencyProperty.RegisterAttached("ScaleValue", typeof(double), typeof(TaskPopupMarginScaler), new PropertyMetadata(1.0, OnPropertyChanged));

    public static void SetVerticalOffset(UIElement element, double value) => element.SetValue(VerticalOffsetProperty, value);
    public static double GetVerticalOffset(UIElement element) => (double)element.GetValue(VerticalOffsetProperty);

    public static void SetHorizontalOffset(UIElement element, double value) => element.SetValue(HorizontalOffsetProperty, value);
    public static double GetHorizontalOffset(UIElement element) => (double)element.GetValue(HorizontalOffsetProperty);

    public static void SetScaleValue(UIElement element, double value) => element.SetValue(ScaleValueProperty, value);
    public static double GetScaleValue(UIElement element) => (double)element.GetValue(ScaleValueProperty);

    private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is FrameworkElement element)
        {
            UpdateMargin(element);
        }
    }

    private static void UpdateMargin(FrameworkElement element)
    {
        double verticalOffset = GetVerticalOffset(element);
        double horizontalOffset = GetHorizontalOffset(element);
        double scaleValue = GetScaleValue(element);

        element.Margin = new Thickness(
            -horizontalOffset * scaleValue,
            -verticalOffset * scaleValue,
            horizontalOffset * scaleValue,
            verticalOffset * scaleValue);
    }
}
