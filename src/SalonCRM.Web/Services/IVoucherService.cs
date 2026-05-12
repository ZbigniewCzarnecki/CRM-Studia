using SalonCRM.Web.Data.Entities;

namespace SalonCRM.Web.Services;

public interface IVoucherService
{
    Task<List<VoucherEntity>> GetAllAsync(CancellationToken ct = default);
    Task<List<VoucherEntity>> GetAvailableAsync(CancellationToken ct = default);
    Task<List<VoucherEntity>> GetForClientAsync(int clientId, CancellationToken ct = default);
    Task<List<VoucherEntity>> GetExpiringAsync(int daysAhead, CancellationToken ct = default);
    Task<int> GetExpiringCountAsync(int daysAhead, CancellationToken ct = default);
    Task<VoucherEntity> CreateAsync(VoucherEntity voucher, CancellationToken ct = default);
    Task UpdateAsync(VoucherEntity voucher, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
}
