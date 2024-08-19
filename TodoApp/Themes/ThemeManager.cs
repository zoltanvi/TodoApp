using Modules.Common.Events;
using Modules.Settings.Contracts.ViewModels;
using Prism.Events;
using System.Windows;
using Application = System.Windows.Application;

namespace TodoApp.Themes;

/// <summary>
/// Responsible for switching between the dark and light resource dictionary, based on the DarkMode setting.
/// </summary>
public class ThemeManager
{
    private readonly MaterialThemeManagerService _materialThemeManagerService;
    private readonly IEventAggregator _eventAggregator;

    private const string DarkTheme = "pack://application:,,,/TodoApp;component/Themes/DarkTheme.xaml";
    private const string LightTheme = "pack://application:,,,/TodoApp;component/Themes/LightTheme.xaml";

    public ThemeManager(MaterialThemeManagerService materialThemeManagerService, IEventAggregator eventAggregator)
    {
        ArgumentNullException.ThrowIfNull(materialThemeManagerService);
        ArgumentNullException.ThrowIfNull(eventAggregator);

        _materialThemeManagerService = materialThemeManagerService;
        _eventAggregator = eventAggregator;
        _materialThemeManagerService.UpdateTheme();

        AppSettings.Instance.ThemeSettings.SettingsChanged += OnThemeSettingsChanged;

        CheckAndSwitchLightAndDark();
    }

    private void OnThemeSettingsChanged(object? sender, SettingsChangedEventArgs e)
    {
        _materialThemeManagerService.UpdateTheme();

        if (e.PropertyName == nameof(ThemeSettings.DarkMode))
        {
            CheckAndSwitchLightAndDark();
        }

        _eventAggregator.GetEvent<ThemeChangedEvent>().Publish();
    }

    private void CheckAndSwitchLightAndDark()
    {
        if (AppSettings.Instance.ThemeSettings.DarkMode)
        {
            ChangeTheme(from: LightTheme, to: DarkTheme);
        }
        else
        {
            ChangeTheme(from: DarkTheme, to: LightTheme);
        }
    }

    private void ChangeTheme(string from, string to)
    {
        Uri oldUri = new Uri(from, UriKind.RelativeOrAbsolute);
        Uri newUri = new Uri(to, UriKind.RelativeOrAbsolute);

        // The Application resources contains all resources that are used
        SearchAndReplaceAll(Application.Current.Resources, oldUri, newUri);
    }

    private void SearchAndReplaceAll(ResourceDictionary rootDictionary, Uri oldDictionary, Uri newDictionary)
    {
        if (rootDictionary.MergedDictionaries.Count > 0)
        {
            ResourceDictionary foundDictionary = rootDictionary.MergedDictionaries
                .FirstOrDefault(i => oldDictionary.AbsoluteUri.EndsWith(i.Source.OriginalString));

            if (foundDictionary != null)
            {
                ResourceDictionary newRes = new ResourceDictionary() { Source = newDictionary };
                int index = rootDictionary.MergedDictionaries.IndexOf(foundDictionary);
                rootDictionary.MergedDictionaries.RemoveAt(index);
                rootDictionary.MergedDictionaries.Insert(index, newRes);
            }

            foreach (ResourceDictionary item in rootDictionary.MergedDictionaries)
            {
                SearchAndReplaceAll(item, oldDictionary, newDictionary);
            }
        }
    }
}