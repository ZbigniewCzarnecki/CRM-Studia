using System.ComponentModel.DataAnnotations;

namespace SalonCRM.Web.Data.Entities;

public class MembershipPackageEntity
{
    public int Id { get; set; }

    [Required, MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(200)]
    public string ServiceName { get; set; } = string.Empty;

    public int TotalEntries { get; set; } = 6;

    public int ValidityMonths { get; set; } = 6;

    public bool IsActive { get; set; } = true;
}
