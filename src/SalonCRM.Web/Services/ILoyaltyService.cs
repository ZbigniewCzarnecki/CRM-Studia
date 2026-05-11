using SalonCRM.Web.Data.Entities;

namespace SalonCRM.Web.Services;

public interface ILoyaltyService
{
    Task<List<LoyaltyStamp>> GetByClientAsync(int clientId, CancellationToken ct = default);
    Task AddStampAsync(int clientId, int? appointmentId, string userId, string note, bool isManual, CancellationToken ct = default);
    Task<int> CountAsync(int clientId, CancellationToken ct = default);
    Task ResetCardAsync(int clientId, CancellationToken ct = default);
}
