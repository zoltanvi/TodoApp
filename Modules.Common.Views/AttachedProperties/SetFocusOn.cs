using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Modules.Common.Views.AttachedProperties;

/// <summary>
/// Allows to set the focus on the FrameworkElement, based on a "trigger" bool.
/// The trigger is a bool value that is triggered when set to true.
/// Setting it to false does not result setting the focus on the element.
/// 
/// If the control is a TextBoxBase and SelectAllOnFocus = true, all text is selected after focusing on the textbox.
/// If the control is a TextBox or a RichTextBox and CaretAtEndOnFocus = true, the caret is positioned at the end of the text when focused.
/// </summary>
public static class SetFocusOn
{
    public static readonly DependencyProperty TriggerProperty = DependencyProperty.RegisterAttached("Trigger", typeof(bool), typeof(SetFocusOn), new PropertyMetadata(false, OnTriggerTriggered));
    public static readonly DependencyProperty SelectAllOnFocusProperty = DependencyProperty.RegisterAttached("SelectAllOnFocus", typeof(bool), typeof(SetFocusOn), new PropertyMetadata(false));
    public static readonly DependencyProperty CaretAtEndOnFocusProperty = DependencyProperty.RegisterAttached("CaretAtEndOnFocus", typeof(bool), typeof(SetFocusOn), new PropertyMetadata(false));

    public static void SetTrigger(DependencyObject element, bool value) => element.SetValue(TriggerProperty, value);
    public static bool GetTrigger(DependencyObject element) => (bool)element.GetValue(TriggerProperty);

    public static void SetSelectAllOnFocus(DependencyObject element, bool value) => element.SetValue(SelectAllOnFocusProperty, value);
    public static bool GetSelectAllOnFocus(DependencyObject element) => (bool)element.GetValue(SelectAllOnFocusProperty);


    public static void SetCaretAtEndOnFocus(DependencyObject element, bool value) => element.SetValue(CaretAtEndOnFocusProperty, value);
    public static bool GetCaretAtEndOnFocus(DependencyObject element) => (bool)element.GetValue(CaretAtEndOnFocusProperty);


    private static void OnTriggerTriggered(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is FrameworkElement element && e.NewValue is true)
        {
            element.Focus();

            if (GetSelectAllOnFocus(element) && element is TextBoxBase textBoxBase)
            {
                textBoxBase.SelectAll();
            }

            if (GetCaretAtEndOnFocus(element))
            {
                if (element is TextBox textBox)
                {
                    textBox.CaretIndex = textBox.Text.Length;
                } 
                else if (element is RichTextBox richTextBox)
                {
                    richTextBox.CaretPosition = richTextBox.CaretPosition.DocumentEnd;
                }
            }
        }
    }
}
