using Modules.Settings.Contracts.Models;

namespace Modules.Settings.Repositories;

public class SettingsRepository : ISettingsRepository
{
    private readonly SettingDbContext _context;

    public SettingsRepository(SettingDbContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        _context = context;
    }

    public void AddSettings(IEnumerable<Setting> settings)
    {
        _context.Settings.AddRange(settings);
        _context.SaveChanges();
    }

    public List<Setting> GetAllSettings() => _context.Settings.ToList();

    public void UpdateSettings(IEnumerable<Setting> settings)
    {
        foreach (var setting in settings)
        {
            var dbSetting = _context.Settings.Find(setting.Key);
            ArgumentNullException.ThrowIfNull(dbSetting);

            dbSetting.Value = setting.Value;
        }

        _context.SaveChanges();
    }
}
