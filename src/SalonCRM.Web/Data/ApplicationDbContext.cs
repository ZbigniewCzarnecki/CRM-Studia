using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SalonCRM.Web.Data.Entities;

namespace SalonCRM.Web.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<ClientEntity> Clients => Set<ClientEntity>();
    public DbSet<ServiceEntity> Services => Set<ServiceEntity>();
    public DbSet<AppointmentEntity> Appointments => Set<AppointmentEntity>();
    public DbSet<SalonSettings> Settings => Set<SalonSettings>();
    public DbSet<LoyaltyStamp> LoyaltyStamps => Set<LoyaltyStamp>();
    public DbSet<UserPermissions> UserPermissions => Set<UserPermissions>();
    public DbSet<MembershipPackageEntity> MembershipPackages => Set<MembershipPackageEntity>();
    public DbSet<ClientMembershipEntity> ClientMemberships => Set<ClientMembershipEntity>();
    public DbSet<VoucherEntity> Vouchers => Set<VoucherEntity>();
}
