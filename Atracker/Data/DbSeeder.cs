// In Data/DbSeeder.cs
using Atracker.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Atracker.Data
{
    public static class DbSeeder
    {
        public static async Task SeedRolesAndAdminAsync(IServiceProvider services)
        {
            // Get the required services
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

            // 1. Create Roles if they don't exist
            string[] roleNames = { "Admin", "Head", "Manager", "Tracker" };
            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // 2. Create Admin User
            if (await userManager.FindByEmailAsync("admin@example.com") == null)
            {
                var adminUser = new ApplicationUser
                {
                    UserName = "admin@example.com",
                    Email = "admin@example.com",
                    FullName = "Admin User",
                    EmailConfirmed = true
                };
                var result = await userManager.CreateAsync(adminUser, "Password@123");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }

            // 3. Create Head User
            if (await userManager.FindByEmailAsync("head@example.com") == null)
            {
                var headUser = new ApplicationUser
                {
                    UserName = "head@example.com",
                    Email = "head@example.com",
                    FullName = "Head Department",
                    EmailConfirmed = true
                };
                var result = await userManager.CreateAsync(headUser, "Password@123");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(headUser, "Head");
                }
            }

            // 4. Create Manager User
            if (await userManager.FindByEmailAsync("manager@example.com") == null)
            {
                var managerUser = new ApplicationUser
                {
                    UserName = "manager@example.com",
                    Email = "manager@example.com",
                    FullName = "Manager One",
                    EmailConfirmed = true
                };
                var result = await userManager.CreateAsync(managerUser, "Password@123");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(managerUser, "Manager");
                }
            }

            // 5. Create Tracker User (Regular User)
            if (await userManager.FindByEmailAsync("tracker1@example.com") == null)
            {
                var trackerUser = new ApplicationUser
                {
                    UserName = "tracker1@example.com",
                    Email = "tracker1@example.com",
                    FullName = "Driver One",
                    EmailConfirmed = true
                };
                var result = await userManager.CreateAsync(trackerUser, "Password@123");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(trackerUser, "Tracker");
                }
            }
        }
    }
}