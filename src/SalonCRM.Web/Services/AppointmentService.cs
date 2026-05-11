using Microsoft.EntityFrameworkCore;
using SalonCRM.Web.Data;
using SalonCRM.Web.Data.Entities;

namespace SalonCRM.Web.Services;

public class AppointmentService : IAppointmentService
{
    private readonly ApplicationDbContext _db;

    public AppointmentService(ApplicationDbContext db) => _db = db;

    public Task<List<AppointmentEntity>> GetAllAsync(CancellationToken ct = default) =>
        _db.Appointments.OrderBy(a => a.Date).ToListAsync(ct);

    public Task<List<AppointmentEntity>> SearchAsync(string query, DateTime? from, DateTime? to, CancellationToken ct = default)
    {
        var q = query.Trim().ToLower();
        var result = _db.Appointments.AsQueryable();

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
        return appt;
    }

    public async Task UpdateAsync(AppointmentEntity appt, CancellationToken ct = default)
    {
        _db.Appointments.Update(appt);
        await _db.SaveChangesAsync(ct);
    }

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
}
