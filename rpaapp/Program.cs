using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using rpaapp.Data;
using rpaapp.Models;
using rpaapp.Repositories;
using Azure.Identity;
using rpaapp.Hubs;

var builder = WebApplication.CreateBuilder(args);
if (builder.Environment.IsProduction()) //azure key vault
{
    builder.Configuration.AddAzureKeyVault(
        new Uri($"https://{builder.Configuration["KeyVaultName"]}.vault.azure.net/"),
        new DefaultAzureCredential());
}

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddScoped<IProcessRepository, ProcessRepository>();

builder.Services.AddIdentity<Writer, IdentityRole<int>>(options =>  
    {
        options.Stores.MaxLengthForKeys = 128;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireLowercase = false;
        options.Password.RequireDigit = false;
    })
    .AddRoles<IdentityRole<int>>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultUI()
    .AddDefaultTokenProviders();
builder.Services.AddControllersWithViews();
builder.Services.AddSignalR();
//builder.Services.AddIdentityServer();

var app = builder.Build();

using (var scope = app.Services.CreateScope()) //initialize async
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        context.Database.Migrate();

        var config = app.Services.GetRequiredService<IConfiguration>();

        var testUserPw = config["NewPw"];
        await DbInitializer.InitializeAsync(context, services, testUserPw);
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

//app.UseIdentityServer();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();
app.MapHub<SDHub>("/SDHub");

app.Run();
