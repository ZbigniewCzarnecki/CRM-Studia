using SalonCRM.Web.Data.Entities;

namespace SalonCRM.Web.Services;

public interface IClientService
{
    Task<List<ClientEntity>> GetAllAsync(CancellationToken ct = default);
    Task<List<ClientEntity>> SearchAsync(string query, CancellationToken ct = default);
    Task<ClientEntity?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<ClientEntity> CreateAsync(ClientEntity client, CancellationToken ct = default);
    Task UpdateAsync(ClientEntity client, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
}
