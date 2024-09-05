using FootBallShop.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using FootBallShop.Service;
using FootBallShop.Settings;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json")
    .Build();

var connectionString = configuration.GetConnectionString("LocalSqlServerConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(connectionString);
});
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.Name = ".AspNetCore.Session";
});

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()  // Add support for roles
    .AddEntityFrameworkStores<AppDbContext>();
builder.Services.AddRazorPages();
builder.Services.Configure<TwilioSettings>(builder.Configuration.GetSection("TwilioSettings"));
builder.Services.AddScoped<ISMSSenderService, SMSSenderService>();

var app = builder.Build();

// Apply pending migrations and seed the database
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();  // Ensure pending migrations are applied
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

    // Seed admin role and user
    await SeedAdminUser(roleManager, userManager);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();  // Ensure session is configured before authentication

app.UseAuthentication();  // Enable authentication
app.UseAuthorization();

app.MapRazorPages();  // Map Razor Pages for Identity

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

/// Ensure the roles exist and the admin user is assigned the "Admin" role
async Task SeedAdminUser(RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager)
{
    // Ensure the Admin role exists
    if (!await roleManager.RoleExistsAsync("Admin"))
    {
        await roleManager.CreateAsync(new IdentityRole("Admin"));
    }

    // Ensure the admin user exists
    var adminUser = await userManager.FindByEmailAsync("philippe@gmail.com");
    if (adminUser == null)
    {
        var newAdminUser = new IdentityUser
        {
            UserName = "philippe@gmail.com",
            Email = "philippe@gmail.com",
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(newAdminUser, "Philo2001@");
        if (result.Succeeded)
        {
            // Assign the Admin role
            await userManager.AddToRoleAsync(newAdminUser, "Admin");
        }
    }
}

