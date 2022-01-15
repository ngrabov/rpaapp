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

            var plist = new List<ProcessType> {
            new ProcessType{ id = 1, name = "IT"},
            new ProcessType{ id = 2, name = "HR"},
            new ProcessType{ id = 3, name = "Skladište"},
            new ProcessType{ id = 4, name = "Logistika"},
            new ProcessType{ id = 5, name = "Transport"},
            new ProcessType{ id = 6, name = "Marketing"},
            new ProcessType{ id = 7, name = "Izvoz"},
            new ProcessType{ id = 8, name = "Veleprodaja"},
            new ProcessType{ id = 9, name = "Režije"},
            new ProcessType{ id = 10, name = "Kamate i krediti"},
            new ProcessType{ id = 11, name = "Leasing"},
            new ProcessType{ id = 12, name = "Špedicija"},
            new ProcessType{ id = 13, name = "Domaća nabava"},
            new ProcessType{ id = 14, name = "Inozemna nabava"},
            new ProcessType{ id = 15, name = "Računi za dugotrajnu imovinu - ostali"},
            new ProcessType{ id = 16, name = "Računi za reprezentaciju - gotovina"},
            new ProcessType{ id = 17, name = "Računi za reprezentaciju - kartica"},
            new ProcessType{ id = 18, name = "Računi za reprezentaciju - transakcijski račun"},
            new ProcessType{ id = 19, name = "Održavanje i osiguravanje vozila"},
            new ProcessType{ id = 20, name = "Računi za trgovačku robu - poslovnice"},
            new ProcessType{ id = 21, name = "Storno računa za trgovačku robu"},
            new ProcessType{ id = 22, name = "Financijska odobrenja i terećenja za robu"},
            new ProcessType{ id = 23, name = "Računi - ostalo"},
            new ProcessType{ id = 24, name = "Uprava"},
            new ProcessType{ id = 25, name = "Računi za reklamacije"},
            new ProcessType{ id = 26, name = "Odobrenja dobavljača za reklamacije"},
            new ProcessType{ id = 27, name = "ACC"},
            new ProcessType{ id = 28, name = "TEC"},
            new ProcessType{ id = 29, name = "Alati"},
            new ProcessType{ id = 30, name = "Servisna oprema"},
            new ProcessType{ id = 31, name = "Računi za kartično poslovanje"},
            new ProcessType{ id = 32, name = "Potrošni materijal"},
            new ProcessType{ id = 33, name = "Stojan"},
            new ProcessType{ id = 34, name = "Visa kartice"},
            new ProcessType{ id = 35, name = "Gospodarski program"},
            new ProcessType{ id = 36, name = "Nabava"},
            new ProcessType{ id = 37, name = "FZO"},
            new ProcessType{ id = 38, name = "Lean"},
            new ProcessType{ id = 39, name = "Kamate i krediti 2"},
            new ProcessType{ id = 40, name = "Maloprodaja"},
            new ProcessType{ id = 41, name = "Zavisni troškovi"},
            new ProcessType{ id = 42, name = "Opći i pravni poslovi"},
            new ProcessType{ id = 43, name = "Web Shop"},
            new ProcessType{ id = 44, name = "Call centar"},
            new ProcessType{ id = 45, name = "Razvoj poslovanja"},
            new ProcessType{ id = 46, name = "Sigurnost"},
            new ProcessType{ id = 47, name = "Analitika"},
            new ProcessType{ id = 48, name = "Industrijska oprema"},
            new ProcessType{ id = 49, name = "Računovodstvo"},
            new ProcessType{ id = 50, name = "Održavanje i investicije"},
            new ProcessType{ id = 51, name = "DTI"},
            new ProcessType{ id = 52, name = "HR-Kupjak"},
            new ProcessType{ id = 53, name = "Stock"},
            new ProcessType{ id = 54, name = "Skladište V2"},
            new ProcessType{ id = 55, name = "Računi prodaja guma"}};

            foreach(var prcs in plist)
            {
                await context.Processes.AddAsync(prcs);
            }

            var roleAdmin = services.GetRequiredService<RoleManager<IdentityRole<int>>>();
            await EnsureRoleAsync(roleAdmin, "Administrator");

            var userAdmin = services.GetRequiredService<UserManager<Writer>>();
            await EnsureAdminAsync(userAdmin, Pw);

            await context.SaveChangesAsync();
        }

        /* public static async Task InitializeAsync(TwoContext context, IServiceProvider services, string Pw)
        {
            await context.Database.EnsureCreatedAsync();

            if(context.Writers.Any()) { return; }

            var roleAdmin = services.GetRequiredService<RoleManager<IdentityRole<int>>>();
            await EnsureRoleAsync(roleAdmin, "Administrator");

            var userAdmin = services.GetRequiredService<UserManager<Writer>>();
            await EnsureAdminAsync(userAdmin, Pw);
        } */

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