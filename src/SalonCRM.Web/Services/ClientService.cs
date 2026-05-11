using Microsoft.EntityFrameworkCore;
using SalonCRM.Web.Data;
using SalonCRM.Web.Data.Entities;

namespace SalonCRM.Web.Services;

public class ClientService : IClientService
{
    private readonly ApplicationDbContext _db;

    public ClientService(ApplicationDbContext db) => _db = db;

    public Task<List<ClientEntity>> GetAllAsync(CancellationToken ct = default) =>
        _db.Clients.OrderBy(c => c.LastName).ThenBy(c => c.FirstName).ToListAsync(ct);

    public Task<List<ClientEntity>> SearchAsync(string query, CancellationToken ct = default)
    {
        var q = query.Trim().ToLower();
        return _db.Clients
            .Where(c => c.LastName.ToLower().Contains(q)
                     || c.FirstName.ToLower().Contains(q)
                     || c.Email.ToLower().Contains(q)
                     || c.PhoneNumber.Contains(q))
            .OrderBy(c => c.LastName)
            .ToListAsync(ct);
    }

    public Task<ClientEntity?> GetByIdAsync(int id, CancellationToken ct = default) =>
        _db.Clients.FindAsync([id], ct).AsTask()!;

    public async Task<ClientEntity> CreateAsync(ClientEntity client, CancellationToken ct = default)
    {
        _db.Clients.Add(client);
        await _db.SaveChangesAsync(ct);
        return client;
    }

    public async Task UpdateAsync(ClientEntity client, CancellationToken ct = default)
    {
        _db.Clients.Update(client);
        await _db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var client = await _db.Clients.FindAsync([id], ct);
        if (client != null)
        {
            _db.Clients.Remove(client);
            await _db.SaveChangesAsync(ct);
        }
    }
}
