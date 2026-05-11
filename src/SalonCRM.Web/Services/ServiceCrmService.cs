using Microsoft.EntityFrameworkCore;
using SalonCRM.Web.Data;
using SalonCRM.Web.Data.Entities;

namespace SalonCRM.Web.Services;

public class ServiceCrmService : IServiceCrmService
{
    private readonly ApplicationDbContext _db;

    public ServiceCrmService(ApplicationDbContext db) => _db = db;

    public Task<List<ServiceEntity>> GetAllAsync(CancellationToken ct = default) =>
        _db.Services.AsNoTracking().OrderBy(s => s.Name).ToListAsync(ct);

    public Task<List<ServiceEntity>> SearchAsync(string query, CancellationToken ct = default)
    {
        var q = query.Trim().ToLower();
        return _db.Services.AsNoTracking()
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

    public Task UpdateAsync(ServiceEntity service, CancellationToken ct = default) =>
        _db.Services
            .Where(s => s.Id == service.Id)
            .ExecuteUpdateAsync(s => s
                .SetProperty(e => e.Name, service.Name)
                .SetProperty(e => e.Description, service.Description)
                .SetProperty(e => e.DurationMinutes, service.DurationMinutes)
                .SetProperty(e => e.Price, service.Price)
                .SetProperty(e => e.Category, service.Category), ct);

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
