using FootBallShop.Models;
using FootBallShop.Service;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ── Load .env into environment variables (dev only) ──────────────────────────
var envFile = Path.Combine(Directory.GetCurrentDirectory(), ".env");
if (File.Exists(envFile))
{
    foreach (var line in File.ReadAllLines(envFile))
    {
        if (string.IsNullOrWhiteSpace(line) || line.TrimStart().StartsWith("#"))
            continue;
        var parts = line.Split('=', 2);
        if (parts.Length == 2)
            Environment.SetEnvironmentVariable(parts[0].Trim(), parts[1].Trim());
    }
}

// ── Services ──────────────────────────────────────────────────────────────────
builder.Services.AddControllersWithViews();

var connectionString =
    Environment.GetEnvironmentVariable("ConnectionStrings__LocalSqlServerConnection")
    ?? builder.Configuration.GetConnectionString("LocalSqlServerConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.Name = ".AspNetCore.Session";
});

builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;
    options.Tokens.AuthenticatorTokenProvider = TokenOptions.DefaultEmailProvider;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<AppDbContext>();

builder.Services.AddTransient<IEmailSender, EmailSenderService>();
builder.Services.AddRazorPages();

var app = builder.Build();

// ── Auto-migrate + seed admin on startup ─────────────────────────────────────
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    try
    {
        dbContext.Database.Migrate(); // Creates DB + all tables if they don't exist
        logger.LogInformation("Database migration applied successfully.");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Database migration failed.");
        throw;
    }

    await SeedAdminUser(roleManager, userManager, logger);
}

// ── HTTP pipeline ─────────────────────────────────────────────────────────────
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();
app.MapRazorPages();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

// ── Admin seeder ──────────────────────────────────────────────────────────────
async Task SeedAdminUser(RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager, ILogger logger)
{
    var adminEmail = Environment.GetEnvironmentVariable("ADMIN_EMAIL");
    var adminPassword = Environment.GetEnvironmentVariable("ADMIN_PASSWORD");

    // 1. Ensure Admin role exists
    if (!await roleManager.RoleExistsAsync("Admin"))
    {
        await roleManager.CreateAsync(new IdentityRole("Admin"));
        logger.LogInformation("Admin role created.");
    }

    // 2. Find or create admin user
    var adminUser = await userManager.FindByEmailAsync(adminEmail);
    if (adminUser == null)
    {
        adminUser = new IdentityUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true  // Skip email confirmation for admin
        };

        var result = await userManager.CreateAsync(adminUser, adminPassword);
        if (result.Succeeded)
        {
            logger.LogInformation("Admin user created: {Email}", adminEmail);
        }
        else
        {
            foreach (var err in result.Errors)
                logger.LogError("Admin creation error: {Code} — {Description}", err.Code, err.Description);
            return;
        }
    }
    else
    {
        // User exists — make sure EmailConfirmed is true
        if (!adminUser.EmailConfirmed)
        {
            adminUser.EmailConfirmed = true;
            await userManager.UpdateAsync(adminUser);
            logger.LogInformation("Admin email confirmed.");
        }
    }

    // 3. Assign Admin role if not already assigned
    if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
    {
        await userManager.AddToRoleAsync(adminUser, "Admin");
        logger.LogInformation("Admin role assigned to {Email}", adminEmail);
    }
    else
    {
        logger.LogInformation("Admin already set up: {Email}", adminEmail);
    }
}