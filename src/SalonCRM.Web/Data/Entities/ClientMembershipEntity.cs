using System.ComponentModel.DataAnnotations;

namespace SalonCRM.Web.Data.Entities;

public class ClientMembershipEntity
{
    public int Id { get; set; }

    public int ClientId { get; set; }
    public ClientEntity Client { get; set; } = null!;

    public int PackageId { get; set; }

    [MaxLength(200)]
    public string PackageName { get; set; } = string.Empty;

    [MaxLength(200)]
    public string ServiceName { get; set; } = string.Empty;

    public int TotalEntries { get; set; }

    public int RemainingEntries { get; set; }

    public DateTime AssignedAt { get; set; } = DateTime.Now;

    public DateTime ExpiresAt { get; set; }

    [MaxLength(500)]
    public string Notes { get; set; } = string.Empty;

    public bool IsExpired => DateTime.Now > ExpiresAt;
    public bool IsExhausted => RemainingEntries <= 0;
    public bool IsActive => !IsExpired && !IsExhausted;
}
