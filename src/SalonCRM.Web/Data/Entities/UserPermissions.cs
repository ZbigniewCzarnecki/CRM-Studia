using System.ComponentModel.DataAnnotations;

namespace SalonCRM.Web.Data.Entities;

public class UserPermissions
{
    [Key]
    public string UserId { get; set; } = "";
    public bool CanManageAppointments { get; set; } = true;
    public bool CanDeleteAppointments { get; set; } = false;
    public bool CanEditClientData { get; set; } = false;
    public bool CanManageServices { get; set; } = false;
    public bool CanViewReports { get; set; } = true;
    public bool CanManageWorkers { get; set; } = false;
    public bool CanManageSettings { get; set; } = false;
    public bool CanManageLoyaltyCards { get; set; } = true;
}
