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

    // ═══ ORDER ID SANITIZATION (STRICT SYNC) ═══
    try
    {
        Console.WriteLine("[DB CLEANUP] Checking for order ID consistency...");
        var allOrders = db.Orders.OrderBy(o => o.CreatedAt).ToList();
        
        if (allOrders.Any())
        {
            Console.WriteLine($"[DB CLEANUP] Found {allOrders.Count} orders. First Order ID: {allOrders.First().Id}");
            
            if (allOrders.First().Id != 1)
            {
                int offset = allOrders.First().Id - 1;
                Console.WriteLine($"[DB CLEANUP] Shifting IDs with offset: {offset}");
                
                foreach (var order in allOrders)
                {
                    var oldId = order.Id;
                    var newId = oldId - offset;

                    db.Database.ExecuteSqlRaw($"UPDATE \"OrderItems\" SET \"OrderId\" = {newId} WHERE \"OrderId\" = {oldId}");
                    db.Database.ExecuteSqlRaw($"UPDATE \"Orders\" SET \"Id\" = {newId} WHERE \"Id\" = {oldId}");
                }

                var maxId = allOrders.Max(o => o.Id) - offset;
                db.Database.ExecuteSqlRaw($"SELECT setval(pg_get_serial_sequence('\"Orders\"', 'Id'), {maxId}, true)");
                
                Console.WriteLine($"[DB CLEANUP] SUCCESS: Shifted {allOrders.Count} orders. Max ID is now {maxId}.");
            }
            else
            {
                Console.WriteLine("[DB CLEANUP] Order ID is already 1. No shifting needed.");
            }
        }
        else
        {
            Console.WriteLine("[DB CLEANUP] No orders found to sanitize.");
            // Reset sequence to 0 just in case there are no orders
            db.Database.ExecuteSqlRaw("SELECT setval(pg_get_serial_sequence('\"Orders\"', 'Id'), 1, false)");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[DB CLEANUP] ERROR: {ex.Message}");
        if (ex.InnerException != null) Console.WriteLine($"[DB CLEANUP] INNER ERROR: {ex.InnerException.Message}");
    }
}

app.Run();
