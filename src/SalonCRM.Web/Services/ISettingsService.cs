using SalonCRM.Web.Data.Entities;

namespace SalonCRM.Web.Services;

public interface ISettingsService
{
    Task<SalonSettings> GetAsync(CancellationToken ct = default);
    Task SaveAsync(SalonSettings settings, CancellationToken ct = default);
}
