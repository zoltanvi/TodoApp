using System.Windows;
using System.Windows.Controls;

namespace Modules.Common.Views.Controls;

/// <summary>
/// A control to dynamically extend the visual tree when needed.
/// Controlling the visibility of the framework elements does not mean they are not part of the visual tree,
/// which could cause performance issues.
/// </summary>
public partial class ShowHideControl : UserControl
{
    public static readonly DependencyProperty CanShowProperty = DependencyProperty.Register(nameof(CanShow), typeof(bool), typeof(ShowHideControl), new PropertyMetadata(true, OnCanShowChanged));
    public static readonly DependencyProperty TemplateToShowProperty = DependencyProperty.Register(nameof(TemplateToShow), typeof(DataTemplate), typeof(ShowHideControl), new PropertyMetadata(default(DataTemplate)));

    // Id for easier debugging
    public string Id { get; set; }

    public bool CanShow
    {
        get { return (bool)GetValue(CanShowProperty); }
        set { SetValue(CanShowProperty, value); }
    }

    public DataTemplate TemplateToShow
    {
        get { return (DataTemplate)GetValue(TemplateToShowProperty); }
        set { SetValue(TemplateToShowProperty, value); }
    }

    private static void OnCanShowChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (ShowHideControl)d;
        var canShow = (bool)e.NewValue;

        control.ContentControlElement.ContentTemplateSelector = new ShowHideControlSelector(control.Id, control.TemplateToShow, canShow);
    }

    public ShowHideControl()
    {
        Loaded += OnLoaded;
        InitializeComponent();
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        Loaded -= OnLoaded;

        ContentControlElement.ContentTemplateSelector = new ShowHideControlSelector(Id, TemplateToShow, CanShow);
    }
}

public class ShowHideControlSelector : DataTemplateSelector
{
    private readonly DataTemplate? _templateToShow;
    private readonly string _id;
    public bool CanShow { get; set; }
    
    public ShowHideControlSelector()
    {
    }

    public ShowHideControlSelector(string id, DataTemplate templateToShow, bool canShow)
    {
        _id = id;
        _templateToShow = templateToShow;
        CanShow = canShow;
    }

    public override DataTemplate? SelectTemplate(object? item, DependencyObject container)
    {
        if (CanShow)
        {
            return _templateToShow;
        }

        return null;
    }
}
