using Microsoft.AspNetCore.Identity;
using rpaapp.Models;
using Microsoft.EntityFrameworkCore;

namespace rpaapp.Data
{
    public static class DbInitializer
    {
        public static async Task InitializeAsync(ApplicationDbContext context, IServiceProvider services, string Pw)
        {
            await context.Database.EnsureCreatedAsync();

            if(context.Writers.Any()) { return; }

            var roleAdmin = services.GetRequiredService<RoleManager<IdentityRole<int>>>();
            await EnsureRoleAsync(roleAdmin, "Administrator");

            var userAdmin = services.GetRequiredService<UserManager<Writer>>();
            await EnsureAdminAsync(userAdmin, Pw);
        }

        private static async Task EnsureRoleAsync(RoleManager<IdentityRole<int>> roleManager, string role)
        {
            var alreadyExists = await roleManager.RoleExistsAsync(role);
            if(alreadyExists) return;

            await roleManager.CreateAsync(new IdentityRole<int>(role));
        }

        private static async Task EnsureAdminAsync(UserManager<Writer> userManager, string userPw)
        {
            var testAdmin = await userManager.Users
                .Where(x => x.UserName == "ngrabovac@tokic.hr")
                .SingleOrDefaultAsync();

            if(testAdmin != null) return;

            testAdmin = new Writer
            {
                UserName = "ngrabovac@tokic.hr",
                Email = "ngrabovac@tokic.hr",  
                LastName = "Grabovac",
                FirstName = "Nikola"
            };
            
            await userManager.CreateAsync(testAdmin, userPw);
            await userManager.AddToRoleAsync(testAdmin, "Administrator");
        }
    }
}