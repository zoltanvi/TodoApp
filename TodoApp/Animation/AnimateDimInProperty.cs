using Modules.Common.Views.Animations;
using System.Windows;

namespace TodoApp.Animation;

public class AnimateDimInProperty : AnimateBaseProperty<AnimateDimInProperty>
{
    protected override async void DoAnimation(FrameworkElement element, bool value, bool firstLoad)
    {
        if (value)
            // Animate in
            await element.DimInAsync(firstLoad ? 0 : FastAnimationDuration, 0.4, 1.0);
        else
            // Animate out
            await element.DimOutAsync(firstLoad ? 0 : FastAnimationDuration, 1.0, 0.4);
    }
}