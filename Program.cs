using Microsoft.EntityFrameworkCore;
using CafeManagement.Data;
using CafeManagement.Services;
using CafeManagement.Models;
using CafeManagement.Models.Enums;

// Load .env file
DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Priority: Environment Variable > appsettings.json
var connectionString = Environment.GetEnvironmentVariable("DATABASE_URL")
    ?? builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<CafeDbContext>(options => 
    options.UseNpgsql(connectionString));

builder.Services.AddSession(options => {
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddControllersWithViews();

// Register Application Services
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<MenuService>();
builder.Services.AddScoped<OrderService>();
builder.Services.AddScoped<DashboardService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseSession();
app.UseAuthorization();

app.UseStatusCodePagesWithReExecute("/Home/PageNotFound");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Runtime Database Initialization & Seeding
using (var scope = app.Services.CreateScope()) 
{
    var db = scope.ServiceProvider.GetRequiredService<CafeDbContext>();
    db.Database.Migrate();

    // ═══ RUNTIME SEEDER (Reads from .env) ═══
    var adminName = Environment.GetEnvironmentVariable("ADMIN_NAME") ?? "Admin";
    var adminEmail = Environment.GetEnvironmentVariable("ADMIN_EMAIL") ?? "admin@cafe.com";
    var adminPass = Environment.GetEnvironmentVariable("ADMIN_PASSWORD") ?? "admin123";
    
    var staffName = Environment.GetEnvironmentVariable("STAFF_NAME") ?? "Staff User";
    var staffEmail = Environment.GetEnvironmentVariable("STAFF_EMAIL") ?? "staff@cafe.com";
    var staffPass = Environment.GetEnvironmentVariable("STAFF_PASSWORD") ?? "staff123";

    if (!db.Users.Any(u => u.Email == adminEmail))
    {
        db.Users.Add(new User {
            Name = adminName,
            Email = adminEmail,
            Password = BCrypt.Net.BCrypt.HashPassword(adminPass),
            Role = UserRole.Admin,
            CreatedAt = DateTime.UtcNow
        });
    }

    if (!db.Users.Any(u => u.Email == staffEmail))
    {
        db.Users.Add(new User {
            Name = staffName,
            Email = staffEmail,
            Password = BCrypt.Net.BCrypt.HashPassword(staffPass),
            Role = UserRole.Staff,
            CreatedAt = DateTime.UtcNow
        });
    }
    db.SaveChanges();
}

app.Run();
