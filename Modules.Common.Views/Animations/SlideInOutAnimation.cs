using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace Modules.Common.Views.Animations;

public static class SlideInOutAnimation
{
    public static readonly DependencyProperty IsAnimatingProperty =
        DependencyProperty.RegisterAttached("IsAnimating", typeof(bool), typeof(SlideInOutAnimation),
            new PropertyMetadata(false, OnIsAnimatingChanged, OnIsAnimatingUpdated));

    public static readonly DependencyProperty DurationProperty =
        DependencyProperty.RegisterAttached("Duration", typeof(TimeSpan), typeof(SlideInOutAnimation),
            new PropertyMetadata(TimeSpan.FromSeconds(5)));

    private static readonly DependencyProperty IsVisibleProperty =
        DependencyProperty.RegisterAttached("IsVisible", typeof(bool), typeof(SlideInOutAnimation), new PropertyMetadata(false));

    private static readonly DependencyProperty AnimationTimerProperty =
        DependencyProperty.RegisterAttached("AnimationTimer", typeof(DispatcherTimer), typeof(SlideInOutAnimation));

    public static readonly DependencyProperty IsClosedProperty =
        DependencyProperty.RegisterAttached("IsClosed", typeof(bool), typeof(SlideInOutAnimation), new PropertyMetadata(false, OnIsClosedChanged));

    /// <summary>
    /// True if this is the very first time the value has been updated
    /// Used to make sure we run the logic at least once during first load
    /// </summary>
    private static readonly Dictionary<WeakReference, bool> AlreadyLoaded = [];

    public static bool GetIsClosed(UIElement element)
    {
        return (bool)element.GetValue(IsClosedProperty);
    }

    public static void SetIsClosed(UIElement element, bool value)
    {
        element.SetValue(IsClosedProperty, value);
    }

    public static bool GetIsAnimating(DependencyObject obj)
    {
        return (bool)obj.GetValue(IsAnimatingProperty);
    }

    public static void SetIsAnimating(DependencyObject obj, bool value)
    {
        obj.SetValue(IsAnimatingProperty, value);
    }

    public static TimeSpan GetDuration(DependencyObject obj)
    {
        return (TimeSpan)obj.GetValue(DurationProperty);
    }

    public static void SetDuration(DependencyObject obj, TimeSpan value)
    {
        obj.SetValue(DurationProperty, value);
    }

    private static bool GetIsVisible(DependencyObject obj)
    {
        return (bool)obj.GetValue(IsVisibleProperty);
    }

    private static void SetIsVisible(DependencyObject obj, bool value)
    {
        obj.SetValue(IsVisibleProperty, value);
    }

    private static DispatcherTimer GetAnimationTimer(DependencyObject obj)
    {
        return (DispatcherTimer)obj.GetValue(AnimationTimerProperty);
    }

    private static void SetAnimationTimer(DependencyObject obj, DispatcherTimer value)
    {
        obj.SetValue(AnimationTimerProperty, value);
    }

    private static void OnIsAnimatingChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        if (sender is FrameworkElement element)
        {
            if ((bool)e.NewValue)
            {
                // If IsAnimating is true, start or reset the animation
                StartOrResetAnimation(element);
            }
        }
    }

    // This method is called even when the value is not changed, just updated.
    private static object OnIsAnimatingUpdated(DependencyObject d, object value)
    {
        if (d is FrameworkElement element)
        {
            var alreadyLoadedRef = AlreadyLoaded.FirstOrDefault(x => Equals(x.Key.Target, element));
            if (alreadyLoadedRef.Key == null)
            {
                var weakReference = new WeakReference(element);
                
                // Flag that we are in first load but have not finished it
                AlreadyLoaded[weakReference] = false;

                void OnLoaded(object loadedObject, RoutedEventArgs e)
                {
                    element.Loaded -= OnLoaded;

                    // Run animation instantly
                    SlideOut(element, 0);
                    
                    AlreadyLoaded[weakReference] = true;
                }

                element.Loaded += OnLoaded;
            }
        }

        return value;
    }

    private static void OnIsClosedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is FrameworkElement element)
        {
            if ((bool)e.NewValue)
            {
                CloseAnimation(element);
            }
        }
    }

    private static void StartOrResetAnimation(FrameworkElement element)
    {
        TimeSpan duration = GetDuration(element);

        // If the element is already visible, reset the timer
        if (GetIsVisible(element))
        {
            var timer = GetAnimationTimer(element);
            if (timer != null)
            {
                timer.Stop();
                timer.Interval = duration;
                timer.Start();
            }
        }
        else
        {
            SlideIn(element);

            // Set the element as visible and start the timer to slide out after the duration
            SetIsVisible(element, true);
            var timer = new DispatcherTimer { Interval = duration };

            timer.Tick += TimerOnTick;
            void TimerOnTick(object? sender, EventArgs e)
            {
                SlideOut(element);
                timer.Stop();
                SetIsVisible(element, false);

                timer.Tick -= TimerOnTick;
            }
            timer.Start();

            // Store the timer in the attached property for future reference
            SetAnimationTimer(element, timer);
        }
    }

    private static void CloseAnimation(FrameworkElement element)
    {
        var timer = GetAnimationTimer(element);
        if (timer != null)
        {
            timer.Stop();
        }
        SlideOut(element);
        SetIsVisible(element, false);
    }

    private static void SlideIn(FrameworkElement element, double duration = 0.5)
    {
        var height = element.ActualHeight;
        var slideInAnimation = new DoubleAnimation
        {
            From = -height,
            To = 0,
            Duration = new Duration(TimeSpan.FromSeconds(duration)),
            EasingFunction = new BackEase { EasingMode = EasingMode.EaseIn }
        };

        element.RenderTransform = new TranslateTransform();
        element.RenderTransform.BeginAnimation(TranslateTransform.YProperty, slideInAnimation);
    }

    private static void SlideOut(FrameworkElement element, double duration = 0.5)
    {
        var height = element.ActualHeight;
        var slideOutAnimation = new DoubleAnimation
        {
            From = 0,
            To = -height,
            Duration = new Duration(TimeSpan.FromSeconds(duration)),
            EasingFunction = new BackEase { EasingMode = EasingMode.EaseOut }
        };

        element.RenderTransform = new TranslateTransform();
        element.RenderTransform.BeginAnimation(TranslateTransform.YProperty, slideOutAnimation);
    }
}