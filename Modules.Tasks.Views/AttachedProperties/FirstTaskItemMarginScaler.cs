using System.Windows;

namespace Modules.Tasks.Views.AttachedProperties;

public static class FirstTaskItemMarginScaler
{
    public static readonly DependencyProperty TopMarginProperty = DependencyProperty.RegisterAttached("TopMargin", typeof(double), typeof(FirstTaskItemMarginScaler), new PropertyMetadata(0.0));
    public static readonly DependencyProperty ScaleValueProperty = DependencyProperty.RegisterAttached("ScaleValue", typeof(double), typeof(FirstTaskItemMarginScaler), new PropertyMetadata(0.0, OnScaleValueChanged));
    public static readonly DependencyProperty IsFirstItemProperty = DependencyProperty.RegisterAttached("IsFirstItem", typeof(bool), typeof(FirstTaskItemMarginScaler), new PropertyMetadata(false, OnIsFirstItemChanged));


    public static void SetTopMargin(DependencyObject element, double value)
    {
        element.SetValue(TopMarginProperty, value);
    }

    public static double GetTopMargin(DependencyObject element)
    {
        return (double)element.GetValue(TopMarginProperty);
    }


    public static void SetScaleValue(DependencyObject element, double value)
    {
        element.SetValue(ScaleValueProperty, value);
    }

    public static double GetScaleValue(DependencyObject element)
    {
        return (double)element.GetValue(ScaleValueProperty);
    }


    public static void SetIsFirstItem(DependencyObject element, bool value)
    {
        element.SetValue(IsFirstItemProperty, value);
    }

    public static bool GetIsFirstItem(DependencyObject element)
    {
        return (bool)element.GetValue(IsFirstItemProperty);
    }

    private static void OnScaleValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is FrameworkElement element)
        {
            UpdateMargin(element);
        }
    }


    private static void OnIsFirstItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is FrameworkElement element)
        {
            UpdateMargin(element);
        }
    }

    private static void UpdateMargin(FrameworkElement element)
    {
        var isFirstItem = GetIsFirstItem(element);
        var topMargin = GetTopMargin(element);
        var scaleValue = GetScaleValue(element);

        var resultMargin = isFirstItem ? topMargin * scaleValue : 0;

        element.Margin = new Thickness(0, resultMargin, 0, 0);
    }
}
