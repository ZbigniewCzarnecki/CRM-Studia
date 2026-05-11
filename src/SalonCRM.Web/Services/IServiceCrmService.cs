using SalonCRM.Web.Data.Entities;

namespace SalonCRM.Web.Services;

public interface IServiceCrmService
{
    Task<List<ServiceEntity>> GetAllAsync(CancellationToken ct = default);
    Task<List<ServiceEntity>> SearchAsync(string query, CancellationToken ct = default);
    Task<ServiceEntity?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<ServiceEntity> CreateAsync(ServiceEntity service, CancellationToken ct = default);
    Task UpdateAsync(ServiceEntity service, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
}
