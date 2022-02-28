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

            var ilist = new List<InvoiceType> {
                new InvoiceType{ id = 1, name = "Stroškovni - Domači", customid = 1},
                new InvoiceType{ id = 2, name = "Blagovni - Tuji (EU)", customid = 2},
                new InvoiceType{ id = 3, name = "Ostali", customid = 4},
                new InvoiceType{ id = 4, name = "Blagovni - Tuji (3.države)", customid = 5},
                new InvoiceType{ id = 5, name = "Blagovni - Domači", customid = 6},
                new InvoiceType{ id = 6, name = "Stroškovni - Tuji (EU)", customid = 7},
                new InvoiceType{ id = 7, name = "Stroškovni - Tuji (3.države)", customid = 8}
            };

            foreach(var invoice in ilist)
            {
                await context.Invoices.AddAsync(invoice);
            }

            var plist = new List<ProcessType> {
            new ProcessType{ id = 1, name = "001 IT", ptid = 1},
            new ProcessType{ id = 2, name = "002 HR", ptid = 2},
            new ProcessType{ id = 3, name = "003 Skladište", ptid = 3},
            new ProcessType{ id = 4, name = "004 Logistika", ptid = 4},
            new ProcessType{ id = 5, name = "005 Transport", ptid = 5},
            new ProcessType{ id = 6, name = "006 Maloprodaja", ptid = 6},
            new ProcessType{ id = 7, name = "007 Marketing", ptid = 7},
            new ProcessType{ id = 8, name = "008 Izvoz", ptid = 8},
            new ProcessType{ id = 9, name = "009 Veleprodaja", ptid = 9},
            new ProcessType{ id = 10, name = "010 Režije", ptid = 10},
            new ProcessType{ id = 11, name = "011 Kamate i krediti", ptid = 11},
            new ProcessType{ id = 12, name = "012 Leasing", ptid = 12},
            new ProcessType{ id = 13, name = "013 Špedicija", ptid = 13},
            new ProcessType{ id = 14, name = "014 Domaća nabava", ptid = 14},
            new ProcessType{ id = 15, name = "015 Inozemna nabava", ptid = 15},
            new ProcessType{ id = 16, name = "016 Računi za dugotrajnu imovinu - ostali", ptid = 16},
            new ProcessType{ id = 17, name = "017 Računi za reprezentaciju - gotovina", ptid = 17},
            new ProcessType{ id = 18, name = "018 Računi za reprezentaciju - kartica", ptid = 18},
            new ProcessType{ id = 19, name = "019 Računi za reprezentaciju - transakcijski račun", ptid = 19},
            new ProcessType{ id = 20, name = "020 Održavanje i osiguravanje vozila", ptid = 20},
            new ProcessType{ id = 21, name = "021 Računi za trgovačku robu - poslovnice", ptid = 21},
            new ProcessType{ id = 22, name = "022 Storno računa za trgovačku robu", ptid = 22},
            new ProcessType{ id = 23, name = "023 Financijska odobrenja i terećenja za robu", ptid = 23},
            new ProcessType{ id = 24, name = "024 Računi - ostalo", ptid = 24},
            new ProcessType{ id = 25, name = "025 Uprava", ptid = 25},
            new ProcessType{ id = 26, name = "026 Računi za reklamacije", ptid = 26},
            new ProcessType{ id = 27, name = "027 Odobrenja dobavljača za reklamacije", ptid = 27},
            new ProcessType{ id = 28, name = "028 ACC", ptid = 28},
            new ProcessType{ id = 29, name = "029 TEC", ptid = 29},
            new ProcessType{ id = 30, name = "030 Alati", ptid = 30},
            new ProcessType{ id = 31, name = "031 Servisna oprema", ptid = 31},
            new ProcessType{ id = 32, name = "032 Računi za kartično poslovanje", ptid = 32},
            new ProcessType{ id = 33, name = "033 Potrošni materijal", ptid = 33},
            new ProcessType{ id = 34, name = "034 Stojan", ptid = 34},
            new ProcessType{ id = 35, name = "035 Visa kartice", ptid = 35},
            new ProcessType{ id = 36, name = "036 Gospodarski program", ptid = 36},
            new ProcessType{ id = 37, name = "037 Nabava", ptid = 37},
            new ProcessType{ id = 38, name = "038 FZO", ptid = 38},
            new ProcessType{ id = 39, name = "039 Lean", ptid = 39},
            new ProcessType{ id = 40, name = "040 Maloprodaja - regija Zapad", ptid = 40},
            new ProcessType{ id = 41, name = "041 Zavisni troškovi", ptid = 41},
            new ProcessType{ id = 42, name = "042 Opći i pravni poslovi", ptid = 42},
            new ProcessType{ id = 43, name = "043 Webshop", ptid = 43},
            new ProcessType{ id = 44, name = "044 Call centar", ptid = 44},
            new ProcessType{ id = 45, name = "045 Razvoj poslovanja", ptid = 45},
            new ProcessType{ id = 46, name = "046 Sigurnost", ptid = 46},
            new ProcessType{ id = 47, name = "047 Analitika", ptid = 47},
            new ProcessType{ id = 48, name = "048 Industrijska oprema", ptid = 48},
            new ProcessType{ id = 49, name = "049 Računi Lean Kaizen", ptid = 49},
            new ProcessType{ id = 50, name = "050 Održavanje i investicije", ptid = 50},
            new ProcessType{ id = 51, name = "051 DTI", ptid = 51},
            new ProcessType{ id = 52, name = "052 HR-Kupjak", ptid = 52},
            new ProcessType{ id = 53, name = "053 Stock", ptid = 53},
            new ProcessType{ id = 54, name = "054 Skladište V2", ptid = 54},
            new ProcessType{ id = 55, name = "055 Računi prodaja guma", ptid = 55},
            new ProcessType{ id = 56, name = "0071 Računi Marketing izravni", ptid = 71}};

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