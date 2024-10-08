﻿using Modules.Settings.Contracts.Models;
using Modules.Settings.Contracts.ViewModels;
using Modules.Settings.Repositories;
using System.Globalization;

namespace Modules.Settings.Services;

public sealed class AppSettingsService : IAppSettingsService
{
    readonly ISettingsRepository _settingsRepository;

    public AppSettingsService(ISettingsRepository settingsRepository)
    {
        ArgumentNullException.ThrowIfNull(settingsRepository);

        _settingsRepository = settingsRepository;   
    }

    public void UpdateAppSettingsFromDatabase(AppSettings appSettings)
    {
        var settings = _settingsRepository.GetAllSettings();

        foreach (var setting in settings)
        {
            SetPropertyValue(appSettings, setting.Key, setting.Value);
        }
    }

    public void UpdateDatabaseFromAppSettings(AppSettings appSettings)
    {
        var settingList = CreateSettingsList(appSettings, string.Empty);
     
        UpdateSettings(settingList);
    }

    /// <summary>
    /// Updates all settings entry in the database if exists,
    /// adds the entry if it not exists.
    /// </summary>
    private void UpdateSettings(List<Setting> settings)
    {
        var existingSettings = _settingsRepository.GetAllSettings();

        var existingSettingsMap =
            existingSettings.ToDictionary(settingsModel => settingsModel.Key);

        var settingsToUpdate = settings
            .Where(s => existingSettingsMap.ContainsKey(s.Key))
            .Where(s => s.Value != existingSettingsMap[s.Key].Value)
            .ToList();

        var settingsToAdd = settings
            .Where(s => !existingSettingsMap.ContainsKey(s.Key))
            .ToList();

        if (settingsToAdd.Count != 0)
        {
            _settingsRepository.AddSettings(settingsToAdd);
        }

        if (settingsToUpdate.Count != 0)
        {
            _settingsRepository.UpdateSettings(settingsToUpdate);
        }
    }

    private static List<Setting> CreateSettingsList(object obj, string currentPath)
    {
        var settings = new List<Setting>();

        foreach (var prop in obj.GetType().GetProperties())
        {
            if (prop.CanRead && prop.CanWrite) // Check if the property has both a getter and a setter
            {
                var propValue = prop.GetValue(obj);

                if (propValue != null)
                {
                    var propPath = string.IsNullOrEmpty(currentPath) ? prop.Name : $"{currentPath}.{prop.Name}";

                    if (prop.PropertyType.IsPrimitive || prop.PropertyType == typeof(string) || prop.PropertyType.IsEnum)
                    {
                        var isDouble = prop.PropertyType == typeof(double);

                        var propValueString = isDouble
                            ? ((double)propValue).ToString(CultureInfo.InvariantCulture)
                            : propValue.ToString();

                        settings.Add(new Setting { Key = propPath, Value = propValueString });
                    }
                    else
                    {
                        settings.AddRange(CreateSettingsList(propValue, propPath));
                    }
                }
            }
        }

        return settings;
    }

    private static void SetPropertyValue(object obj, string path, string value)
    {
        var parts = path.Split('.');
        var currentObj = obj;

        for (var i = 0; i < parts.Length; i++)
        {
            var part = parts[i];
            var propInfo = currentObj.GetType().GetProperty(part);

            if (propInfo == null)
            {
#if DEBUG
                //throw new ArgumentException($"Property '{part}' not found in {currentObj.GetType().Name}");
#endif
                continue;
            }

            if (i == parts.Length - 1)
            {
                // Last part of the path - set the value
                var propType = propInfo.PropertyType;

                TryConvert(value, propType, out var typedValue);

                //object typedValue = Convert.ChangeType(value, propType);
                propInfo.SetValue(currentObj, typedValue);
            }
            else
            {
                // Not the last part - check for null and create an instance if necessary
                var propValue = propInfo.GetValue(currentObj);
                if (propValue == null)
                {
                    propValue = Activator.CreateInstance(propInfo.PropertyType);
                    propInfo.SetValue(currentObj, propValue);
                }

                currentObj = propValue;
            }
        }
    }

    private static void TryConvert(string value, Type propType, out object result)
    {
        result = null;

        if (propType == typeof(string))
        {
            result = value;
        }
        else if (propType.IsEnum)
        {
            var enumValue = Enum.Parse(propType, value);

            if (Enum.IsDefined(propType, enumValue))
            {
                result = enumValue;
            }
            else
            {
                throw new ArgumentException($"Cannot convert unknown datatype. The datatype: [{propType.Name}].");
            }
        }
        else if (propType == typeof(double) &&
            double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var doubleValue))
        {
            result = doubleValue;
        }
        else if (propType == typeof(int) && int.TryParse(value, out var intValue))
        {
            result = intValue;
        }
        else if (propType == typeof(bool) && bool.TryParse(value, out var boolValue))
        {
            result = boolValue;
        }
        else
        {
            throw new ArgumentException($"Cannot convert unknown datatype. The datatype: [{propType.Name}].");
        }
    }
}
