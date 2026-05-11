namespace SalonCRM.Web.Data.Entities;

public class SalonSettings
{
    public int Id { get; set; } = 1;
    public string SalonName { get; set; } = "SalonCRM";
    public int WorkDayStart { get; set; } = 8;
    public int WorkDayEnd { get; set; } = 20;
    public int SlotMinutes { get; set; } = 30;
    public int LoyaltyStampsForReward { get; set; } = 10;
    public string Currency { get; set; } = "PLN";
    public string PhoneNumber { get; set; } = "";
    public string Address { get; set; } = "";
}
