using Microsoft.AspNetCore.Identity;

namespace SalonCRM.Web.Data;

public class ApplicationUser : IdentityUser
{
    public string DisplayName { get; set; } = string.Empty;
}
