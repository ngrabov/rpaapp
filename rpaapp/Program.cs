using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using rpaapp.Data;
using rpaapp.Models;
using rpaapp.Repositories;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");/* 
var TwoString = builder.Configuration.GetConnectionString("TwoConnection"); */
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));/* 
builder.Services.AddDbContext<TwoContext>(options => 
    options.UseSqlite(TwoString)); */
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddScoped<IProcessRepository, ProcessRepository>();

builder.Services.AddIdentity<Writer, IdentityRole<int>>(options =>  options.Stores.MaxLengthForKeys = 128)
    .AddRoles<IdentityRole<int>>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultUI()
    .AddDefaultTokenProviders();
/* builder.Services.AddIdentity<Writer, IdentityRole<int>>(options =>  options.Stores.MaxLengthForKeys = 128)
    .AddRoles<IdentityRole<int>>()
    .AddEntityFrameworkStores<TwoContext>()
    .AddDefaultUI()
    .AddDefaultTokenProviders(); */
builder.Services.AddControllersWithViews();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        //var twocontext = services.GetRequiredService<TwoContext>();
        context.Database.Migrate();

        var config = app.Services.GetRequiredService<IConfiguration>();

        var testUserPw = config["NewPw"];
        await DbInitializer.InitializeAsync(context, services, testUserPw);
        //await DbInitializer.InitializeAsync(twocontext, services, testUserPw);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred creating the DB.");
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
