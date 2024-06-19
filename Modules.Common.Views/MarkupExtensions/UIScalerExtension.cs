using Microsoft.Extensions.DependencyInjection;
using Modules.Common.Services;
using System.Windows.Markup;

namespace Modules.Common.Views.MarkupExtensions;

public class UIScalerExtension : MarkupExtension
{
    public override object? ProvideValue(IServiceProvider serviceProvider)
    {
        // Replace this with your method call to get the UIScaler instance
        return serviceProvider.GetService<IUIScaler>();
    }
}