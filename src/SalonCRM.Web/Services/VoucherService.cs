using Microsoft.EntityFrameworkCore;
using SalonCRM.Web.Data;
using SalonCRM.Web.Data.Entities;

namespace SalonCRM.Web.Services;

public class VoucherService : IVoucherService
{
    private readonly ApplicationDbContext _db;
    public VoucherService(ApplicationDbContext db) => _db = db;

    public Task<List<VoucherEntity>> GetAllAsync(CancellationToken ct = default) =>
        _db.Vouchers.AsNoTracking().OrderByDescending(v => v.CreatedAt).ToListAsync(ct);

    public Task<List<VoucherEntity>> GetAvailableAsync(CancellationToken ct = default) =>
        _db.Vouchers.AsNoTracking()
            .Where(v => !v.IsUsed && v.AppointmentId == null && v.ExpiresAt > DateTime.Now)
            .OrderBy(v => v.ExpiresAt)
            .ToListAsync(ct);

    public Task<List<VoucherEntity>> GetExpiringAsync(int daysAhead, CancellationToken ct = default)
    {
        var cutoff = DateTime.Now.AddDays(daysAhead);
        return _db.Vouchers.AsNoTracking()
            .Where(v => !v.IsUsed && v.ExpiresAt > DateTime.Now && v.ExpiresAt <= cutoff)
            .OrderBy(v => v.ExpiresAt)
            .ToListAsync(ct);
    }

    public Task<int> GetExpiringCountAsync(int daysAhead, CancellationToken ct = default)
    {
        var cutoff = DateTime.Now.AddDays(daysAhead);
        return _db.Vouchers.CountAsync(v => !v.IsUsed && v.ExpiresAt > DateTime.Now && v.ExpiresAt <= cutoff, ct);
    }

    public async Task<VoucherEntity> CreateAsync(VoucherEntity voucher, CancellationToken ct = default)
    {
        voucher.CreatedAt = DateTime.Now;
        _db.Vouchers.Add(voucher);
        await _db.SaveChangesAsync(ct);
        return voucher;
    }

    public Task UpdateAsync(VoucherEntity voucher, CancellationToken ct = default) =>
        _db.Vouchers
            .Where(v => v.Id == voucher.Id)
            .ExecuteUpdateAsync(s => s
                .SetProperty(v => v.Code, voucher.Code)
                .SetProperty(v => v.RecipientName, voucher.RecipientName)
                .SetProperty(v => v.Type, voucher.Type)
                .SetProperty(v => v.AmountValue, voucher.AmountValue)
                .SetProperty(v => v.ServiceName, voucher.ServiceName)
                .SetProperty(v => v.ExpiresAt, voucher.ExpiresAt)
                .SetProperty(v => v.Notes, voucher.Notes)
                .SetProperty(v => v.IsUsed, voucher.IsUsed), ct);

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var v = await _db.Vouchers.FindAsync([id], ct);
        if (v != null) { _db.Vouchers.Remove(v); await _db.SaveChangesAsync(ct); }
    }
}
