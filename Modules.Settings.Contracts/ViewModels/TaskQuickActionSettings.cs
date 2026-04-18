namespace Modules.Settings.Contracts.ViewModels;

public class TaskQuickActionSettings : SettingsBase
{
    // CheckboxEnabled is not on quick actions panel, so it shouldn't be included in AnyEnabled
    public bool AnyEnabled => Enabled &&
        (ReminderEnabled ||
        ColorEnabled ||
        BackgroundColorEnabled ||
        BorderColorEnabled ||
        TagEnabled ||
        PinEnabled ||
        DetailsEnabled ||
        TrashEnabled);

    public bool Enabled { get; set; } = true;
    public bool ReminderEnabled { get; set; }
    public bool ColorEnabled { get; set; } = true;
    public bool BackgroundColorEnabled { get; set; }
    public bool BorderColorEnabled { get; set; }
    public bool TagEnabled { get; set; } = true;
    public bool PinEnabled { get; set; } = true;
    public bool DetailsEnabled { get; set; } = true;
    public bool TrashEnabled { get; set; } = true;
    
    // Not on quick actions panel
    public bool CheckboxEnabled { get; set; } = true;
}
