using Microsoft.EntityFrameworkCore;
using SalonCRM.Web.Data;
using SalonCRM.Web.Data.Entities;

namespace SalonCRM.Web.Services;

public class SaleService : ISaleService
{
    private readonly ApplicationDbContext _db;

    public SaleService(ApplicationDbContext db) => _db = db;

    public Task<List<SaleEntity>> GetAllAsync(CancellationToken ct = default) =>
        _db.Sales.AsNoTracking().Include(s => s.Items)
            .OrderByDescending(s => s.SaleDate)
            .ToListAsync(ct);

    public Task<List<SaleEntity>> GetByDateRangeAsync(DateTime from, DateTime to, CancellationToken ct = default) =>
        _db.Sales.AsNoTracking().Include(s => s.Items)
            .Where(s => s.SaleDate >= from && s.SaleDate <= to)
            .OrderByDescending(s => s.SaleDate)
            .ToListAsync(ct);

    public Task<SaleEntity?> GetByIdAsync(int id, CancellationToken ct = default) =>
        _db.Sales.Include(s => s.Items).FirstOrDefaultAsync(s => s.Id == id, ct);

    public async Task<SaleEntity> CreateAsync(SaleEntity sale, CancellationToken ct = default)
    {
        sale.TotalGrossAmount = sale.Items.Sum(i => i.LineGrossAmount);
        sale.TotalNetAmount = Math.Round(sale.Items.Sum(i => i.LineNetAmount), 2);
        sale.TotalTaxAmount = sale.TotalGrossAmount - sale.TotalNetAmount;
        _db.Sales.Add(sale);
        await _db.SaveChangesAsync(ct);
        return sale;
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var sale = await _db.Sales.Include(s => s.Items).FirstOrDefaultAsync(s => s.Id == id, ct);
        if (sale != null) { _db.Sales.Remove(sale); await _db.SaveChangesAsync(ct); }
    }

    public Task<decimal> GetRevenueAsync(DateTime from, DateTime to, CancellationToken ct = default) =>
        _db.Sales
            .Where(s => s.SaleDate >= from && s.SaleDate <= to)
            .SumAsync(s => s.TotalGrossAmount, ct);

    public async Task<(decimal Gross, decimal Net, decimal Tax, int Count)> GetSummaryAsync(DateTime from, DateTime to, CancellationToken ct = default)
    {
        var data = await _db.Sales
            .Where(s => s.SaleDate >= from && s.SaleDate <= to)
            .Select(s => new { s.TotalGrossAmount, s.TotalNetAmount, s.TotalTaxAmount })
            .ToListAsync(ct);
        return (
            data.Sum(s => s.TotalGrossAmount),
            data.Sum(s => s.TotalNetAmount),
            data.Sum(s => s.TotalTaxAmount),
            data.Count
        );
    }
}
