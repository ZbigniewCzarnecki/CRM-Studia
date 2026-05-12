using SalonCRM.Web.Data.Entities;

namespace SalonCRM.Web.Services;

public interface IMembershipService
{
    Task<List<MembershipPackageEntity>> GetAllPackagesAsync(CancellationToken ct = default);
    Task<MembershipPackageEntity> CreatePackageAsync(MembershipPackageEntity pkg, CancellationToken ct = default);
    Task UpdatePackageAsync(MembershipPackageEntity pkg, CancellationToken ct = default);
    Task DeletePackageAsync(int id, CancellationToken ct = default);

    Task<List<ClientMembershipEntity>> GetClientMembershipsAsync(int clientId, CancellationToken ct = default);
    Task<List<ClientMembershipEntity>> GetActiveClientMembershipsAsync(int clientId, CancellationToken ct = default);
    Task<ClientMembershipEntity> AssignAsync(int clientId, int packageId, string notes, CancellationToken ct = default);
    Task UseEntryAsync(int membershipId, CancellationToken ct = default);
    Task ExtendAsync(int membershipId, int additionalMonths, string reason, CancellationToken ct = default);
}
