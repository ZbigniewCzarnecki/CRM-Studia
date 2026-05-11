using SalonCRM.Web.Data.Entities;

namespace SalonCRM.Web.Services;

public interface IAppointmentService
{
    Task<List<AppointmentEntity>> GetAllAsync(CancellationToken ct = default);
    Task<List<AppointmentEntity>> SearchAsync(string query, DateTime? from, DateTime? to, CancellationToken ct = default);
    Task<AppointmentEntity?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<AppointmentEntity> CreateAsync(AppointmentEntity appt, CancellationToken ct = default);
    Task UpdateAsync(AppointmentEntity appt, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
    Task<int> CountUpcomingAsync(CancellationToken ct = default);
    Task<List<AppointmentEntity>> GetByDateAsync(DateTime date, CancellationToken ct = default);
    Task UpdateStatusAsync(int id, AppointmentStatus status, CancellationToken ct = default);
    Task<decimal> GetRevenueAsync(DateTime from, DateTime to, CancellationToken ct = default);
}
