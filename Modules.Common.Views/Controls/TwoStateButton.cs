using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Modules.Common.Views.Controls;

public class TwoStateButton : Button
{
    public static readonly DependencyProperty IsState1ActiveProperty = DependencyProperty.Register(nameof(IsState1Active), typeof(bool), typeof(TwoStateButton), new PropertyMetadata(false, OnIsState1ActiveChanged));

    public static readonly DependencyProperty ContentForState1Property = DependencyProperty.Register(nameof(ContentForState1), typeof(object), typeof(TwoStateButton), new PropertyMetadata(default(object)));
    public static readonly DependencyProperty ContentForState2Property = DependencyProperty.Register(nameof(ContentForState2), typeof(object), typeof(TwoStateButton), new PropertyMetadata(default(object)));

    public static readonly DependencyProperty CommandForState1Property = DependencyProperty.Register(nameof(CommandForState1), typeof(ICommand), typeof(TwoStateButton), new PropertyMetadata(default(ICommand)));
    public static readonly DependencyProperty CommandForState2Property = DependencyProperty.Register(nameof(CommandForState2), typeof(ICommand), typeof(TwoStateButton), new PropertyMetadata(default(ICommand)));

    public bool IsState1Active
    {
        get => (bool)GetValue(IsState1ActiveProperty);
        set => SetValue(IsState1ActiveProperty, value);
    }

    public object ContentForState1
    {
        get => (object)GetValue(ContentForState1Property);
        set => SetValue(ContentForState1Property, value);
    }

    public object ContentForState2
    {
        get => (object)GetValue(ContentForState2Property);
        set => SetValue(ContentForState2Property, value);
    }

    public ICommand CommandForState1
    {
        get => (ICommand)GetValue(CommandForState1Property);
        set => SetValue(CommandForState1Property, value);
    }

    public ICommand CommandForState2
    {
        get => (ICommand)GetValue(CommandForState2Property);
        set => SetValue(CommandForState2Property, value);
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

        if (IsState1Active && CommandForState1?.CanExecute(null) == true)
        {
            CommandForState1.Execute(null);
        }
        else if (!IsState1Active && CommandForState2?.CanExecute(null) == true)
        {
            CommandForState2.Execute(null);
        }
    }

    private void UpdateContent()
    {
        Content = IsState1Active ? ContentForState1 : ContentForState2;
    }
}
