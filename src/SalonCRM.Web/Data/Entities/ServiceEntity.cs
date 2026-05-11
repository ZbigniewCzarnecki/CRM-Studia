using System.ComponentModel.DataAnnotations;

namespace SalonCRM.Web.Data.Entities;

public class ServiceEntity
{
    public int Id { get; set; }

    [Required, MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string Description { get; set; } = string.Empty;

    [Range(1, 480)]
    public int DurationMinutes { get; set; }

    [Range(0.01, 10000)]
    public decimal Price { get; set; }

    [MaxLength(100)]
    public string Category { get; set; } = string.Empty;
}
