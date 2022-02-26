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

            var lyts = new List<LayoutConfig> { new LayoutConfig{Id = 1, Color = "#034791", isVisible = true, pngname = "tokic.png"}};

            foreach(var it in lyts)
            {
                await context.Layouts.AddAsync(it);
            }

            var plist = new List<ProcessType> {
            new ProcessType{ id = 1, name = "001 IT"},
            new ProcessType{ id = 2, name = "002 HR"},
            new ProcessType{ id = 3, name = "003 Skladište"},
            new ProcessType{ id = 4, name = "004 Logistika"},
            new ProcessType{ id = 5, name = "005 Transport"},
            new ProcessType{ id = 56, name = "006 Maloprodaja"},
            new ProcessType{ id = 6, name = "007 Marketing"},
            new ProcessType{ id = 7, name = "008 Izvoz"},
            new ProcessType{ id = 8, name = "009 Veleprodaja"},
            new ProcessType{ id = 9, name = "010 Režije"},
            new ProcessType{ id = 10, name = "011 Kamate i krediti"},
            new ProcessType{ id = 39, name = "011 Kamate i krediti 2"},
            new ProcessType{ id = 11, name = "012 Leasing"},
            new ProcessType{ id = 12, name = "013 Špedicija"},
            new ProcessType{ id = 13, name = "014 Domaća nabava"},
            new ProcessType{ id = 14, name = "015 Inozemna nabava"},
            new ProcessType{ id = 15, name = "016 Računi za dugotrajnu imovinu - ostali"},
            new ProcessType{ id = 16, name = "017 Računi za reprezentaciju - gotovina"},
            new ProcessType{ id = 17, name = "018 Računi za reprezentaciju - kartica"},
            new ProcessType{ id = 18, name = "019 Računi za reprezentaciju - transakcijski račun"},
            new ProcessType{ id = 19, name = "020 Održavanje i osiguravanje vozila"},
            new ProcessType{ id = 20, name = "021 Računi za trgovačku robu - poslovnice"},
            new ProcessType{ id = 21, name = "022 Storno računa za trgovačku robu"},
            new ProcessType{ id = 22, name = "023 Financijska odobrenja i terećenja za robu"},
            new ProcessType{ id = 23, name = "024 Računi - ostalo"},
            new ProcessType{ id = 24, name = "025 Uprava"},
            new ProcessType{ id = 25, name = "026 Računi za reklamacije"},
            new ProcessType{ id = 26, name = "027 Odobrenja dobavljača za reklamacije"},
            new ProcessType{ id = 27, name = "028 ACC"},
            new ProcessType{ id = 28, name = "029 TEC"},
            new ProcessType{ id = 29, name = "030 Alati"},
            new ProcessType{ id = 30, name = "031 Servisna oprema"},
            new ProcessType{ id = 31, name = "032 Računi za kartično poslovanje"},
            new ProcessType{ id = 32, name = "033 Potrošni materijal"},
            new ProcessType{ id = 33, name = "034 Stojan"},
            new ProcessType{ id = 34, name = "035 Visa kartice"},
            new ProcessType{ id = 35, name = "036 Gospodarski program"},
            new ProcessType{ id = 36, name = "037 Nabava"},
            new ProcessType{ id = 37, name = "038 FZO"},
            new ProcessType{ id = 38, name = "039 Lean"},
            new ProcessType{ id = 40, name = "040 Maloprodaja - regija Zapad"},
            new ProcessType{ id = 41, name = "041 Zavisni troškovi"},
            new ProcessType{ id = 42, name = "042 Opći i pravni poslovi"},
            new ProcessType{ id = 43, name = "043 Webshop"},
            new ProcessType{ id = 44, name = "044 Call centar"},
            new ProcessType{ id = 45, name = "045 Razvoj poslovanja"},
            new ProcessType{ id = 46, name = "046 Sigurnost"},
            new ProcessType{ id = 47, name = "047 Analitika"},
            new ProcessType{ id = 48, name = "048 Industrijska oprema"},
            new ProcessType{ id = 49, name = "500 Računovodstvo"},
            new ProcessType{ id = 50, name = "050 Održavanje i investicije"},
            new ProcessType{ id = 51, name = "051 DTI"},
            new ProcessType{ id = 52, name = "052 HR-Kupjak"},
            new ProcessType{ id = 53, name = "053 Stock"},
            new ProcessType{ id = 54, name = "054 Skladište V2"},
            new ProcessType{ id = 55, name = "055 Računi prodaja guma"},
            new ProcessType{ id = 57, name = "0071 Računi Marketing izravni"}};

            foreach(var prcs in plist)
            {
                await context.Processes.AddAsync(prcs);
            }

            var roleAdmin = services.GetRequiredService<RoleManager<IdentityRole<int>>>();
            await EnsureRoleAsync(roleAdmin, "Administrator");

            var manager = services.GetRequiredService<RoleManager<IdentityRole<int>>>();
            await EnsureRoleAsync(manager, "Manager");

            var userAdmin = services.GetRequiredService<UserManager<Writer>>();
            await EnsureAdminAsync(userAdmin, Pw);

            await context.SaveChangesAsync();
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