using Microsoft.AspNetCore.Identity;

namespace SalonCRM.Web.Data;

public static class SeedData
{
    public static async Task SeedAdminAsync(IServiceProvider services, IConfiguration config)
    {
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

        // Migracja nazwy roli Staff → Pracownik
        var staffRole = await roleManager.FindByNameAsync("Staff");
        if (staffRole != null)
        {
            staffRole.Name = "Pracownik";
            staffRole.NormalizedName = "PRACOWNIK";
            await roleManager.UpdateAsync(staffRole);
        }

        foreach (var role in new[] { "Admin", "Pracownik" })
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
        }

        var email = config["SeedAdmin:Email"] ?? "admin@salon.pl";
        var password = config["SeedAdmin:Password"] ?? "Admin@123";
        var displayName = config["SeedAdmin:DisplayName"] ?? "Administrator";

        if (await userManager.FindByEmailAsync(email) == null)
        {
            var admin = new ApplicationUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true,
                DisplayName = displayName
            };
            var result = await userManager.CreateAsync(admin, password);
            if (result.Succeeded)
                await userManager.AddToRoleAsync(admin, "Admin");
        }
    }
}
