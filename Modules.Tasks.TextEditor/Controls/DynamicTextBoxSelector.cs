using System.Windows;
using System.Windows.Controls;

namespace Modules.Tasks.TextEditor.Controls;

public class DynamicTextBoxSelector : DataTemplateSelector
{
    public override DataTemplate? SelectTemplate(object? item, DependencyObject container)
    {
        if (container is not FrameworkElement element || item == null)
        {
            return null;
        }

        if (item is DynamicTextBoxViewModel viewModel)
        {
            if (viewModel.IsPlainTextMode)
            {
                return element.FindResource("PlainTextTemplate") as DataTemplate;
            } 

            return element.FindResource("RichTextTemplate") as DataTemplate;
        }

        throw new ApplicationException($"Item is not a {nameof(DynamicTextBoxViewModel)}");
    }
}