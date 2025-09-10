using System;
using Microsoft.AspNetCore.Identity;
using todo.Entities;

namespace todo.Data;

public class Seed
{

    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        if (!await roleManager.RoleExistsAsync("Admin"))
            await roleManager.CreateAsync(new IdentityRole("Admin"));

        if (!await roleManager.RoleExistsAsync("User"))
            await roleManager.CreateAsync(new IdentityRole("User"));

        var adminUser = await userManager.FindByEmailAsync("admin@admin.com");
        if (adminUser == null)
        {
            adminUser = new User
            {
                UserName = "admin@admin.com",
                Email = "admin@admin.com",
                FirstName = "Admin",
                LastName = "Admin"
            };
            await userManager.CreateAsync(adminUser, "Pa$$w0rd");
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }
    }
}

