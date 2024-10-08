﻿using Modules.Settings.Contracts.Models;

namespace Modules.Settings.Repositories;

public interface ISettingsRepository
{
    List<Setting> GetAllSettings();
    void AddSettings(IEnumerable<Setting> settings);
    void UpdateSettings(IEnumerable<Setting> settings);
}
