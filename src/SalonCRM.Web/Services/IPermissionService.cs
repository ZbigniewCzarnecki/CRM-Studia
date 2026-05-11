using System.Security.Claims;
using SalonCRM.Web.Data.Entities;

namespace SalonCRM.Web.Services;

public enum Permission
{
    ManageAppointments,
    DeleteAppointments,
    EditClientData,
    ManageServices,
    ViewReports,
    ManageWorkers,
    ManageSettings,
    ManageLoyaltyCards
}

public interface IPermissionService
{
    Task<bool> HasAsync(ClaimsPrincipal user, Permission permission, CancellationToken ct = default);
    Task<UserPermissions> GetOrCreateAsync(string userId, CancellationToken ct = default);
    Task SaveAsync(UserPermissions perms, CancellationToken ct = default);
}
