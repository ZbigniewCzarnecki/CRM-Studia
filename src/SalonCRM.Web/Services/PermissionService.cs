using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using SalonCRM.Web.Data;
using SalonCRM.Web.Data.Entities;

namespace SalonCRM.Web.Services;

public class PermissionService : IPermissionService
{
    private readonly ApplicationDbContext _db;

    public PermissionService(ApplicationDbContext db) => _db = db;

    public async Task<bool> HasAsync(ClaimsPrincipal user, Permission permission, CancellationToken ct = default)
    {
        if (user.IsInRole("Admin")) return true;

        var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return false;

        var perms = await _db.UserPermissions.AsNoTracking()
            .FirstOrDefaultAsync(p => p.UserId == userId, ct);

        if (perms == null) return false;

        return permission switch
        {
            Permission.ManageAppointments => perms.CanManageAppointments,
            Permission.DeleteAppointments => perms.CanDeleteAppointments,
            Permission.EditClientData => perms.CanEditClientData,
            Permission.ManageServices => perms.CanManageServices,
            Permission.ViewReports => perms.CanViewReports,
            Permission.ManageWorkers => perms.CanManageWorkers,
            Permission.ManageSettings => perms.CanManageSettings,
            Permission.ManageLoyaltyCards => perms.CanManageLoyaltyCards,
            _ => false
        };
    }

    public async Task<UserPermissions> GetOrCreateAsync(string userId, CancellationToken ct = default)
    {
        var perms = await _db.UserPermissions.AsNoTracking()
            .FirstOrDefaultAsync(p => p.UserId == userId, ct);

        if (perms != null) return perms;

        var def = new UserPermissions { UserId = userId };
        _db.UserPermissions.Add(def);
        await _db.SaveChangesAsync(ct);
        return def;
    }

    public Task SaveAsync(UserPermissions perms, CancellationToken ct = default) =>
        _db.UserPermissions
            .Where(p => p.UserId == perms.UserId)
            .ExecuteUpdateAsync(u => u
                .SetProperty(p => p.CanManageAppointments, perms.CanManageAppointments)
                .SetProperty(p => p.CanDeleteAppointments, perms.CanDeleteAppointments)
                .SetProperty(p => p.CanEditClientData, perms.CanEditClientData)
                .SetProperty(p => p.CanManageServices, perms.CanManageServices)
                .SetProperty(p => p.CanViewReports, perms.CanViewReports)
                .SetProperty(p => p.CanManageWorkers, perms.CanManageWorkers)
                .SetProperty(p => p.CanManageSettings, perms.CanManageSettings)
                .SetProperty(p => p.CanManageLoyaltyCards, perms.CanManageLoyaltyCards), ct);
}
