﻿namespace Modules.Settings.Contracts.ViewModels;

public class SessionSettings : SettingsBase
{
    public double SideMenuWidth { get; set; } // 0 = closed by default
    public bool SideMenuOpen { get; set; }
    public int ActiveCategoryId { get; set; } = 1;
    public int ActiveNoteId { get; set; }
    public int ActiveSettingsCategoryId { get; set; } = 1;
}
