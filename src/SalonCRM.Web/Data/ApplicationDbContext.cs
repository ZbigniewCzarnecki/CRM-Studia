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
}
