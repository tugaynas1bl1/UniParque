using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using UniParque_Domain.Entities;

namespace UniParque_Infrastructure.Persistence;

public class RoleSeeder
{
    public static async Task SeedRolesAsync(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();

        var roles = new[] { "Admin", "Manager", "User" };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
        }

        var adminEmail = "tugaynasibli@gmail.com";
        var adminPassword = "tug@ynsVdm1n";

        if (await userManager.FindByEmailAsync(adminEmail) is null)
        {
            var admin = new AppUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                FirstName = "Tugay",
                LastName = "Nasibli",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = null
            };

            var result = await userManager.CreateAsync(admin, adminPassword);

            if (result.Succeeded)
                await userManager.AddToRoleAsync(admin, "Admin");
        }
    }
}
