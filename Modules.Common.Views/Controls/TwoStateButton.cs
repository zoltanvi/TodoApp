using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Modules.Common.Views.Controls;

public class TwoStateButton : Button
{
    public static readonly DependencyProperty IsInSecondStateProperty = DependencyProperty.Register(nameof(IsInSecondState), typeof(bool), typeof(TwoStateButton), new PropertyMetadata(false, OnIsState1ActiveChanged));

    public static readonly DependencyProperty FirstContentProperty = DependencyProperty.Register(nameof(FirstContent), typeof(object), typeof(TwoStateButton), new PropertyMetadata(default(object)));
    public static readonly DependencyProperty SecondContentProperty = DependencyProperty.Register(nameof(SecondContent), typeof(object), typeof(TwoStateButton), new PropertyMetadata(default(object)));

    public static readonly DependencyProperty FirstCommandProperty = DependencyProperty.Register(nameof(FirstCommand), typeof(ICommand), typeof(TwoStateButton), new PropertyMetadata(default(ICommand)));
    public static readonly DependencyProperty SecondCommandProperty = DependencyProperty.Register(nameof(SecondCommand), typeof(ICommand), typeof(TwoStateButton), new PropertyMetadata(default(ICommand)));
    public static readonly DependencyProperty FirstToolTipProperty = DependencyProperty.Register(nameof(FirstToolTip), typeof(object), typeof(TwoStateButton), new PropertyMetadata(default(object)));
    public static readonly DependencyProperty SecondToolTipProperty = DependencyProperty.Register(nameof(SecondToolTip), typeof(object), typeof(TwoStateButton), new PropertyMetadata(default(object)));

    public bool IsInSecondState
    {
        get => (bool)GetValue(IsInSecondStateProperty);
        set => SetValue(IsInSecondStateProperty, value);
    }

    public object FirstContent
    {
        get => (object)GetValue(FirstContentProperty);
        set => SetValue(FirstContentProperty, value);
    }

    public object SecondContent
    {
        get => (object)GetValue(SecondContentProperty);
        set => SetValue(SecondContentProperty, value);
    }

    public ICommand FirstCommand
    {
        get => (ICommand)GetValue(FirstCommandProperty);
        set => SetValue(FirstCommandProperty, value);
    }

    public ICommand SecondCommand
    {
        get => (ICommand)GetValue(SecondCommandProperty);
        set => SetValue(SecondCommandProperty, value);
    }

    public object FirstToolTip
    {
        get => (object)GetValue(FirstToolTipProperty);
        set => SetValue(FirstToolTipProperty, value);
    }

    public object SecondToolTip
    {
        get => (object)GetValue(SecondToolTipProperty);
        set => SetValue(SecondToolTipProperty, value);
    }

    public TwoStateButton()
    {
        Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        Loaded -= OnLoaded;

        UpdateContent();
    }

    private static void OnIsState1ActiveChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var button = (TwoStateButton)d;
        button.UpdateContent();
    }

    protected override void OnClick()
    {
        base.OnClick();

        if (IsInSecondState && SecondCommand?.CanExecute(null) == true)
        {
            SecondCommand.Execute(null);
        }
        else if (!IsInSecondState && FirstCommand?.CanExecute(null) == true)
        {
            FirstCommand.Execute(null);
        }
    }

    private void UpdateContent()
    {
        Content = IsInSecondState ? SecondContent : FirstContent;
        ToolTip = IsInSecondState ? SecondToolTip : FirstToolTip;
    }
}
