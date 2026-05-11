namespace SalonCRM.Web.Data.Entities;

public class LoyaltyStamp
{
    public int Id { get; set; }
    public int ClientId { get; set; }
    public ClientEntity Client { get; set; } = null!;
    public DateTime AwardedAt { get; set; } = DateTime.Now;
    public int? AppointmentId { get; set; }
    public string AddedByUserId { get; set; } = "";
    public string Note { get; set; } = "";
    public bool IsManual { get; set; }
}
