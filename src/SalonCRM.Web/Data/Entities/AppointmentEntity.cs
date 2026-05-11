using System.ComponentModel.DataAnnotations;

namespace SalonCRM.Web.Data.Entities;

public class AppointmentEntity
{
    public int Id { get; set; }

    [Required, MaxLength(200)]
    public string ClientName { get; set; } = string.Empty;

    [Required, MaxLength(200)]
    public string ServiceName { get; set; } = string.Empty;

    public DateTime Date { get; set; }

    [Range(0, 100000)]
    public decimal TotalAmount { get; set; }
}
