using Microsoft.EntityFrameworkCore;
using SalonCRM.Web.Data;
using SalonCRM.Web.Data.Entities;

namespace SalonCRM.Web.Services;

public class ServiceCrmService : IServiceCrmService
{
    private readonly ApplicationDbContext _db;

    public ServiceCrmService(ApplicationDbContext db) => _db = db;

    public Task<List<ServiceEntity>> GetAllAsync(CancellationToken ct = default) =>
        _db.Services.OrderBy(s => s.Name).ToListAsync(ct);

    public Task<List<ServiceEntity>> SearchAsync(string query, CancellationToken ct = default)
    {
        var q = query.Trim().ToLower();
        return _db.Services
            .Where(s => s.Name.ToLower().Contains(q) || s.Description.ToLower().Contains(q))
            .OrderBy(s => s.Name)
            .ToListAsync(ct);
    }

    public Task<ServiceEntity?> GetByIdAsync(int id, CancellationToken ct = default) =>
        _db.Services.FindAsync([id], ct).AsTask()!;

    public async Task<ServiceEntity> CreateAsync(ServiceEntity service, CancellationToken ct = default)
    {
        _db.Services.Add(service);
        await _db.SaveChangesAsync(ct);
        return service;
    }

    public async Task UpdateAsync(ServiceEntity service, CancellationToken ct = default)
    {
        _db.Services.Update(service);
        await _db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var service = await _db.Services.FindAsync([id], ct);
        if (service != null)
        {
            _db.Services.Remove(service);
            await _db.SaveChangesAsync(ct);
        }
    }
}
