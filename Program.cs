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

    // ═══ ORDER ID SANITIZATION ═══
    // If there are orders but they start from a high number (like 11), shift them to start from 1
    try
    {
        var allOrders = await db.Orders.OrderBy(o => o.CreatedAt).ToListAsync();
        if (allOrders.Any() && allOrders.First().Id != 1)
        {
            int offset = allOrders.First().Id - 1;
            foreach (var order in allOrders)
            {
                var oldId = order.Id;
                var newId = oldId - offset;

                // Update both Order and its Items using raw SQL to bypass EF tracking for ID changes
                await db.Database.ExecuteSqlRawAsync($"UPDATE \"OrderItems\" SET \"OrderId\" = {newId} WHERE \"OrderId\" = {oldId}");
                await db.Database.ExecuteSqlRawAsync($"UPDATE \"Orders\" SET \"Id\" = {newId} WHERE \"Id\" = {oldId}");
            }

            // Reset the sequence so next order is max(Id) + 1
            var maxId = allOrders.Max(o => o.Id) - offset;
            await db.Database.ExecuteSqlRawAsync($"SELECT setval(pg_get_serial_sequence('\"Orders\"', 'Id'), {maxId}, true)");
            
            Console.WriteLine($"[DB CLEANUP] Successfully shifted {allOrders.Count} orders and reset sequence to {maxId}.");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[DB CLEANUP] Sequence reset skipped: {ex.Message}");
    }
}

app.Run();
