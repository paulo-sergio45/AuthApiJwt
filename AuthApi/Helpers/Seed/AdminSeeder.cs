using AuthApi.Entities;
using AuthApi.Models.Dtos;
using Microsoft.AspNetCore.Identity;

namespace AuthApi.Helpers.Seed
{
    public class AdminSeeder
    {
        public static async Task SeedAsync(WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

            const string adminRole = "Admin";

            List<RegisterDto> users = [new RegisterDto("admin", "admin@teste.com", "", "Admin@Admin123")];

            await SeedRoles(roleManager);

            await SeedUsersAdmin(userManager, users, adminRole);

        }

        public static async Task SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            string[] roles = { "Admin", "User" };
            foreach (var name in roles)
            {
                if (!await roleManager.RoleExistsAsync(name))
                    await roleManager.CreateAsync(new IdentityRole(name));
            }
        }

        public static async Task SeedUsersAdmin(UserManager<User> userManager, List<RegisterDto> users, string adminRole)
        {
            foreach (var user in users)
            {

                var adminUser = await userManager.FindByEmailAsync(user.Email);
                if (adminUser == null)
                {
                    adminUser = new User
                    {
                        UserName = user.Email,
                        Email = user.Email,
                        EmailConfirmed = true
                    };

                    var result = await userManager.CreateAsync(adminUser, user.Password);
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(adminUser, adminRole);
                    }
                }

            }
        }
    }
}