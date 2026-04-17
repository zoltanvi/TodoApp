namespace Modules.Settings.Contracts.ViewModels;

public class SessionSettings : SettingsBase
{
    public double SideMenuWidth { get; set; } // 0 = closed by default
    public bool SideMenuOpen { get; set; }
    public int ActiveCategoryId { get; set; } = 1;
    public int ActiveNoteId { get; set; }
    public int ActiveSettingsCategoryId { get; set; } = 1;
    public string ExpandedCategoryIds { get; set; } = string.Empty;

    public HashSet<int> GetExpandedCategoryIds()
    {
        if (string.IsNullOrWhiteSpace(ExpandedCategoryIds))
            return [];

        return ExpandedCategoryIds
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(s => int.TryParse(s.Trim(), out var id) ? id : -1)
            .Where(id => id > 0)
            .ToHashSet();
    }

    public void SetExpandedCategoryIds(IEnumerable<int> ids)
    {
        ExpandedCategoryIds = string.Join(",", ids);
    }
}
