using System.ComponentModel.DataAnnotations;

namespace SalonCRM.Web.Data.Entities;

public class VoucherUseEntity
{
    public int Id { get; set; }

    public int VoucherId { get; set; }

    public DateTime UsedAt { get; set; } = DateTime.Now;

    public int? AppointmentId { get; set; }

    [MaxLength(200)]
    public string ClientName { get; set; } = string.Empty;

    [MaxLength(200)]
    public string ServiceName { get; set; } = string.Empty;

    public decimal? AmountDeducted { get; set; }

    public decimal? BalanceAfter { get; set; }
}
