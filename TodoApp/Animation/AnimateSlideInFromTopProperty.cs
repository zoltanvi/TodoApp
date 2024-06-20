using Modules.Common.Views.Animations;
using System.Windows;

namespace TodoApp.Animation;

public class AnimateSlideInFromTopProperty : AnimateBaseProperty<AnimateSlideInFromTopProperty>
{
    protected override async void DoAnimation(FrameworkElement element, bool value, bool firstLoad)
    {
        if (value)
            // Animate in
            await element.SlideInFromTopAsync(firstLoad ? 0 : 1);
        else
            // Animate out
            await element.SlideOutToTopAsync(firstLoad ? 0 : 0.5f);
    }
}