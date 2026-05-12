using System.ComponentModel.DataAnnotations;

namespace SalonCRM.Web.Data.Entities;

public enum VoucherType { Amount, Service }

public class VoucherEntity
{
    public int Id { get; set; }

    [Required, MaxLength(50)]
    public string Code { get; set; } = string.Empty;

    [MaxLength(200)]
    public string RecipientName { get; set; } = string.Empty;

    public VoucherType Type { get; set; }

    public decimal? AmountValue { get; set; }

    [MaxLength(200)]
    public string ServiceName { get; set; } = string.Empty;

    public DateTime ExpiresAt { get; set; }

    public bool IsUsed { get; set; }

    public DateTime? UsedAt { get; set; }

    [MaxLength(300)]
    public string UsedInAppointment { get; set; } = string.Empty;

    [MaxLength(500)]
    public string Notes { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public int? AppointmentId { get; set; }

    public bool IsExpired => DateTime.Now > ExpiresAt;
    public bool IsActive => !IsUsed && !IsExpired;
    public int DaysUntilExpiry => (int)(ExpiresAt - DateTime.Now).TotalDays;
}
