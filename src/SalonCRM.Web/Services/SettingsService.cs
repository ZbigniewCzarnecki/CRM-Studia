using Microsoft.EntityFrameworkCore;
using SalonCRM.Web.Data;
using SalonCRM.Web.Data.Entities;

namespace SalonCRM.Web.Services;

public class SettingsService : ISettingsService
{
    private readonly ApplicationDbContext _db;

    public SettingsService(ApplicationDbContext db) => _db = db;

    public async Task<SalonSettings> GetAsync(CancellationToken ct = default)
    {
        var settings = await _db.Settings.AsNoTracking().FirstOrDefaultAsync(ct);
        if (settings != null) return settings;

        var def = new SalonSettings { Id = 1 };
        _db.Settings.Add(def);
        await _db.SaveChangesAsync(ct);
        return def;
    }

    public Task SaveAsync(SalonSettings settings, CancellationToken ct = default) =>
        _db.Settings
            .Where(s => s.Id == settings.Id)
            .ExecuteUpdateAsync(u => u
                .SetProperty(s => s.SalonName, settings.SalonName)
                .SetProperty(s => s.WorkDayStart, settings.WorkDayStart)
                .SetProperty(s => s.WorkDayEnd, settings.WorkDayEnd)
                .SetProperty(s => s.SlotMinutes, settings.SlotMinutes)
                .SetProperty(s => s.LoyaltyStampsForReward, settings.LoyaltyStampsForReward)
                .SetProperty(s => s.Currency, settings.Currency)
                .SetProperty(s => s.PhoneNumber, settings.PhoneNumber)
                .SetProperty(s => s.Address, settings.Address), ct);
}
