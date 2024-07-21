using Modules.Common.DataModels;
using System.Windows;
using System.Windows.Controls;

namespace Modules.Common.Views.Controls;

public class TagControl : Label
{
    public static readonly DependencyProperty TagTitleProperty = DependencyProperty.Register(nameof(TagTitle), typeof(string), typeof(TagControl), new PropertyMetadata());
    public static readonly DependencyProperty TagColorProperty = DependencyProperty.Register(nameof(TagColor), typeof(TagPresetColor), typeof(TagControl), new PropertyMetadata());

    public string TagTitle
    {
        get { return (string)GetValue(TagTitleProperty); }
        set { SetValue(TagTitleProperty, value); }
    }

    public TagPresetColor TagColor
    {
        get { return (TagPresetColor)GetValue(TagColorProperty); }
        set { SetValue(TagColorProperty, value); }
    }
}
