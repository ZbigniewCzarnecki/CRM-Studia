using Microsoft.EntityFrameworkCore;
using SalonCRM.Web.Data;
using SalonCRM.Web.Data.Entities;

namespace SalonCRM.Web.Services;

public class AppointmentService : IAppointmentService
{
    private readonly ApplicationDbContext _db;
    private readonly ILoyaltyService _loyalty;
    private readonly IMembershipService _membership;

    public AppointmentService(ApplicationDbContext db, ILoyaltyService loyalty, IMembershipService membership)
    {
        _db = db;
        _loyalty = loyalty;
        _membership = membership;
    }

    public Task<List<AppointmentEntity>> GetAllAsync(CancellationToken ct = default) =>
        _db.Appointments.AsNoTracking().OrderBy(a => a.Date).ToListAsync(ct);

    public Task<List<AppointmentEntity>> SearchAsync(string query, DateTime? from, DateTime? to, CancellationToken ct = default)
    {
        var q = query.Trim().ToLower();
        var result = _db.Appointments.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(query))
            result = result.Where(a => a.ClientName.ToLower().Contains(q) || a.ServiceName.ToLower().Contains(q));

        if (from.HasValue) result = result.Where(a => a.Date >= from.Value);
        if (to.HasValue) result = result.Where(a => a.Date <= to.Value);

        return result.OrderBy(a => a.Date).ToListAsync(ct);
    }

    public Task<AppointmentEntity?> GetByIdAsync(int id, CancellationToken ct = default) =>
        _db.Appointments.FindAsync([id], ct).AsTask()!;

    public async Task<AppointmentEntity> CreateAsync(AppointmentEntity appt, CancellationToken ct = default)
    {
        _db.Appointments.Add(appt);
        await _db.SaveChangesAsync(ct);

        if (appt.VoucherId.HasValue)
        {
            var v = await _db.Vouchers.FindAsync([appt.VoucherId.Value], ct);
            if (v != null && v.AppointmentId == null && !v.IsUsed)
            {
                v.AppointmentId = appt.Id;
                if (v.Type == VoucherType.Amount)
                {
                    v.IsUsed = true;
                    v.UsedAt = DateTime.Now;
                    v.UsedInAppointment = $"{appt.ClientName} — {appt.ServiceName}";
                }
                await _db.SaveChangesAsync(ct);
            }
        }

        return appt;
    }

    public Task UpdateAsync(AppointmentEntity appt, CancellationToken ct = default) =>
        _db.Appointments
            .Where(a => a.Id == appt.Id)
            .ExecuteUpdateAsync(s => s
                .SetProperty(a => a.ClientName, appt.ClientName)
                .SetProperty(a => a.ServiceName, appt.ServiceName)
                .SetProperty(a => a.Date, appt.Date)
                .SetProperty(a => a.TotalAmount, appt.TotalAmount)
                .SetProperty(a => a.DurationMinutes, appt.DurationMinutes)
                .SetProperty(a => a.Status, appt.Status)
                .SetProperty(a => a.Notes, appt.Notes)
                .SetProperty(a => a.ClientMembershipId, appt.ClientMembershipId)
                .SetProperty(a => a.VoucherId, appt.VoucherId), ct);

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var appt = await _db.Appointments.FindAsync([id], ct);
        if (appt != null)
        {
            _db.Appointments.Remove(appt);
            await _db.SaveChangesAsync(ct);
        }
    }

    public Task<int> CountUpcomingAsync(CancellationToken ct = default) =>
        _db.Appointments.CountAsync(a => a.Date >= DateTime.Now, ct);

    public Task<List<AppointmentEntity>> GetByDateAsync(DateTime date, CancellationToken ct = default) =>
        _db.Appointments.AsNoTracking()
            .Where(a => a.Date.Date == date.Date)
            .OrderBy(a => a.Date)
            .ToListAsync(ct);

    public Task<List<AppointmentEntity>> GetByRangeAsync(DateTime from, DateTime to, CancellationToken ct = default) =>
        _db.Appointments.AsNoTracking()
            .Where(a => a.Date.Date >= from.Date && a.Date.Date <= to.Date)
            .OrderBy(a => a.Date)
            .ToListAsync(ct);

    public async Task UpdateStatusAsync(int id, AppointmentStatus status, CancellationToken ct = default)
    {
        await _db.Appointments
            .Where(a => a.Id == id)
            .ExecuteUpdateAsync(s => s.SetProperty(a => a.Status, status), ct);

        if (status == AppointmentStatus.Completed)
        {
            var appt = await _db.Appointments.AsNoTracking().FirstOrDefaultAsync(a => a.Id == id, ct);
            if (appt != null)
            {
                var client = await _db.Clients.AsNoTracking()
                    .FirstOrDefaultAsync(c => (c.FirstName + " " + c.LastName) == appt.ClientName, ct);
                if (client != null)
                    await _loyalty.AddStampAsync(client.Id, id, "", "", false, ct);

                if (appt.ClientMembershipId.HasValue)
                    await _membership.UseEntryAsync(appt.ClientMembershipId.Value, ct);

                if (appt.VoucherId.HasValue)
                {
                    var v = await _db.Vouchers.FindAsync([appt.VoucherId.Value], ct);
                    if (v != null && !v.IsUsed)
                    {
                        v.IsUsed = true;
                        v.UsedAt = DateTime.Now;
                        v.UsedInAppointment = $"{appt.ClientName} — {appt.ServiceName}";
                        await _db.SaveChangesAsync(ct);
                    }
                }
            }
        }
    }

    public Task<List<AppointmentEntity>> GetByClientNameAsync(string clientName, CancellationToken ct = default) =>
        _db.Appointments.AsNoTracking()
            .Where(a => a.ClientName == clientName)
            .OrderByDescending(a => a.Date)
            .ToListAsync(ct);

    public async Task<decimal> GetRevenueAsync(DateTime from, DateTime to, CancellationToken ct = default)
    {
        var sum = await _db.Appointments
            .Where(a => a.Status == AppointmentStatus.Completed && a.Date >= from && a.Date <= to)
            .SumAsync(a => (double?)a.TotalAmount, ct);
        return (decimal)(sum ?? 0);
    }
}
