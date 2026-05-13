using SalonCRM.Web.Data.Entities;

namespace SalonCRM.Web.Services;

public interface ISaleService
{
    Task<List<SaleEntity>> GetAllAsync(CancellationToken ct = default);
    Task<List<SaleEntity>> GetByDateRangeAsync(DateTime from, DateTime to, CancellationToken ct = default);
    Task<SaleEntity?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<SaleEntity> CreateAsync(SaleEntity sale, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
    Task<decimal> GetRevenueAsync(DateTime from, DateTime to, CancellationToken ct = default);
    Task<(decimal Gross, decimal Net, decimal Tax, int Count)> GetSummaryAsync(DateTime from, DateTime to, CancellationToken ct = default);
}
