using Microsoft.EntityFrameworkCore;
using SalonCRM.Web.Data;
using SalonCRM.Web.Data.Entities;

namespace SalonCRM.Web.Services;

public class MembershipService : IMembershipService
{
    private readonly ApplicationDbContext _db;

    public MembershipService(ApplicationDbContext db) => _db = db;

    public Task<List<MembershipPackageEntity>> GetAllPackagesAsync(CancellationToken ct = default) =>
        _db.MembershipPackages.AsNoTracking().OrderBy(p => p.Name).ToListAsync(ct);

    public async Task<MembershipPackageEntity> CreatePackageAsync(MembershipPackageEntity pkg, CancellationToken ct = default)
    {
        _db.MembershipPackages.Add(pkg);
        await _db.SaveChangesAsync(ct);
        return pkg;
    }

    public Task UpdatePackageAsync(MembershipPackageEntity pkg, CancellationToken ct = default) =>
        _db.MembershipPackages
            .Where(p => p.Id == pkg.Id)
            .ExecuteUpdateAsync(s => s
                .SetProperty(p => p.Name, pkg.Name)
                .SetProperty(p => p.ServiceName, pkg.ServiceName)
                .SetProperty(p => p.TotalEntries, pkg.TotalEntries)
                .SetProperty(p => p.ValidityMonths, pkg.ValidityMonths)
                .SetProperty(p => p.IsActive, pkg.IsActive), ct);

    public async Task DeletePackageAsync(int id, CancellationToken ct = default)
    {
        var pkg = await _db.MembershipPackages.FindAsync([id], ct);
        if (pkg != null) { _db.MembershipPackages.Remove(pkg); await _db.SaveChangesAsync(ct); }
    }

    public Task<List<ClientMembershipEntity>> GetClientMembershipsAsync(int clientId, CancellationToken ct = default) =>
        _db.ClientMemberships.AsNoTracking()
            .Where(m => m.ClientId == clientId)
            .OrderByDescending(m => m.AssignedAt)
            .ToListAsync(ct);

    public async Task<List<ClientMembershipEntity>> GetActiveClientMembershipsAsync(int clientId, CancellationToken ct = default)
    {
        var all = await GetClientMembershipsAsync(clientId, ct);
        return all.Where(m => m.IsActive).ToList();
    }

    public async Task<ClientMembershipEntity> AssignAsync(int clientId, int packageId, string notes, CancellationToken ct = default)
    {
        var pkg = await _db.MembershipPackages.FindAsync([packageId], ct)
            ?? throw new InvalidOperationException("Package not found");

        var membership = new ClientMembershipEntity
        {
            ClientId = clientId,
            PackageId = packageId,
            PackageName = pkg.Name,
            ServiceName = pkg.ServiceName,
            TotalEntries = pkg.TotalEntries,
            RemainingEntries = pkg.TotalEntries,
            AssignedAt = DateTime.Now,
            ExpiresAt = DateTime.Now.AddMonths(pkg.ValidityMonths),
            Notes = notes
        };

        _db.ClientMemberships.Add(membership);
        await _db.SaveChangesAsync(ct);
        return membership;
    }

    public async Task UseEntryAsync(int membershipId, CancellationToken ct = default)
    {
        var m = await _db.ClientMemberships.FindAsync([membershipId], ct);
        if (m != null && m.RemainingEntries > 0)
        {
            m.RemainingEntries--;
            await _db.SaveChangesAsync(ct);
        }
    }

    public async Task ExtendAsync(int membershipId, int additionalMonths, string reason, CancellationToken ct = default)
    {
        var m = await _db.ClientMemberships.FindAsync([membershipId], ct);
        if (m == null) return;
        m.ExpiresAt = m.ExpiresAt.AddMonths(additionalMonths);
        if (!string.IsNullOrWhiteSpace(reason))
            m.Notes = string.IsNullOrWhiteSpace(m.Notes)
                ? $"Przedłużono o {additionalMonths} mies.: {reason}"
                : m.Notes + $"\nPrzedłużono o {additionalMonths} mies.: {reason}";
        await _db.SaveChangesAsync(ct);
    }
}
