﻿using Microsoft.Xaml.Behaviors;
using System.Windows;
using System.Windows.Controls;

namespace Modules.Common.Views.AttachedProperties;

public class PutCaretAtTheEndRTBBehavior : Behavior<UIElement>
{
    private RichTextBox _richTextBox;

    protected override void OnAttached()
    {
        base.OnAttached();


        if (AssociatedObject is RichTextBox richTextBox)
        {
            _richTextBox = richTextBox;
            _richTextBox.GotFocus += RichTextBox_GotFocus;
        }
    }

    protected override void OnDetaching()
    {
        if (_richTextBox != null)
        {
            _richTextBox.GotFocus -= RichTextBox_GotFocus;
        }

        base.OnDetaching();
    }

    private void RichTextBox_GotFocus(object sender, RoutedEventArgs e)
    {
        _richTextBox.CaretPosition = _richTextBox.Document.ContentEnd;
    }
}
