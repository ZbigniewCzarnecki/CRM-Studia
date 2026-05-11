using Microsoft.EntityFrameworkCore;
using SalonCRM.Web.Data;
using SalonCRM.Web.Data.Entities;

namespace SalonCRM.Web.Services;

public class LoyaltyService : ILoyaltyService
{
    private readonly ApplicationDbContext _db;

    public LoyaltyService(ApplicationDbContext db) => _db = db;

    public Task<List<LoyaltyStamp>> GetByClientAsync(int clientId, CancellationToken ct = default) =>
        _db.LoyaltyStamps.AsNoTracking()
            .Where(s => s.ClientId == clientId)
            .OrderBy(s => s.AwardedAt)
            .ToListAsync(ct);

    public async Task AddStampAsync(int clientId, int? appointmentId, string userId, string note, bool isManual, CancellationToken ct = default)
    {
        _db.LoyaltyStamps.Add(new LoyaltyStamp
        {
            ClientId = clientId,
            AppointmentId = appointmentId,
            AddedByUserId = userId,
            Note = note,
            IsManual = isManual,
            AwardedAt = DateTime.Now
        });
        await _db.SaveChangesAsync(ct);
    }

    public Task<int> CountAsync(int clientId, CancellationToken ct = default) =>
        _db.LoyaltyStamps.CountAsync(s => s.ClientId == clientId, ct);

    public Task ResetCardAsync(int clientId, CancellationToken ct = default) =>
        _db.LoyaltyStamps
            .Where(s => s.ClientId == clientId)
            .ExecuteDeleteAsync(ct);
}
