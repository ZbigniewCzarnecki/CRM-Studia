using System.ComponentModel.DataAnnotations;

namespace SalonCRM.Web.Data.Entities;

public class ClientEntity
{
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    public string LastName { get; set; } = string.Empty;

    [MaxLength(20)]
    public string PhoneNumber { get; set; } = string.Empty;

    [EmailAddress, MaxLength(200)]
    public string Email { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string Notes { get; set; } = string.Empty;

    [MaxLength(450)]
    public string? CreatedByUserId { get; set; }

    public bool ConsentEmail { get; set; }
    public bool ConsentSms { get; set; }
}
