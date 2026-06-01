using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Unyo.Models;

namespace Unyo.Data;

public static class SeedData
{
    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        var context = serviceProvider.GetRequiredService<AppDbContext>();
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<User>>();

        context.Database.Migrate();

        string[] roleNames = ["Admin", "Vendor", "User"];
        foreach (var roleName in roleNames)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
                await roleManager.CreateAsync(new IdentityRole(roleName));
        }

        var adminEmail = "admin@unyo.com";
        if (await userManager.FindByEmailAsync(adminEmail) == null)
        {
            var admin = new User
            {
                UserName = "admin",
                Email = adminEmail,
                FirstName = "Sistem",
                LastName = "Administrator",
                BirthDate = new DateTime(2000, 1, 1),
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(admin, "Admin@123!");
            if (result.Succeeded)
                await userManager.AddToRoleAsync(admin, "Admin");
        }

        var vendorEmail = "concerts.srl@unyo.com";
        if (await userManager.FindByEmailAsync(vendorEmail) == null)
        {
            var vendor = new User
            {
                UserName = "vendor_test",
                Email = vendorEmail,
                FirstName = "Mihai",
                LastName = "Organizatorul",
                BirthDate = new DateTime(1990, 5, 15),
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(vendor, "Vendor@123!");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(vendor, "Vendor");
            }
        }

        if (!context.Categories.Any())
        {
            context.Categories.AddRange(
                new Category { Name = "Concerte" },
                new Category { Name = "Conferințe" },
                new Category { Name = "Teatru" }
            );
            await context.SaveChangesAsync();
        }
    }
}