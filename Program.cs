using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.DataProtection;
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

builder.Services.AddDataProtection()
    .PersistKeysToDbContext<CafeDbContext>();

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

    // ═══ HARD RESET (Requested by USER) ═══
    Console.WriteLine("[HARD RESET] Starting full system wipe...");
    
    // 1. Clear all orders first (to avoid FK constraints)
    db.Database.ExecuteSqlRaw("TRUNCATE TABLE \"OrderItems\", \"Orders\" RESTART IDENTITY CASCADE");
    
    // 2. Remove all users except Admin and Staff
    var testUsers = db.Users.Where(u => u.Email != adminEmail && u.Email != staffEmail).ToList();
    if (testUsers.Any())
    {
        Console.WriteLine($"[HARD RESET] Removing {testUsers.Count} test users...");
        db.Users.RemoveRange(testUsers);
    }

    // 3. Reset table statuses
    db.Database.ExecuteSqlRaw("UPDATE \"CafeTables\" SET \"Status\" = 0"); // 0 = Available
    db.SaveChanges();

    // ═══ TABLE SEEDER (Sync to exactly 8 tables) ═══
    var currentTables = db.CafeTables.OrderBy(t => t.TableNumber).ToList();
    if (currentTables.Count != 8)
    {
        Console.WriteLine($"[DB SEED] Syncing tables... Current: {currentTables.Count}, Target: 8");
        
        if (currentTables.Count < 8)
        {
            // Add missing tables
            int startFrom = currentTables.Any() ? currentTables.Max(t => t.TableNumber) + 1 : 1;
            for (int i = startFrom; i <= 8; i++)
            {
                db.CafeTables.Add(new CafeTable { TableNumber = i, Capacity = 4, Status = TableStatus.Available });
            }
        }
        else
        {
            // Remove extra tables (highest numbers first)
            var toRemove = currentTables.OrderByDescending(t => t.TableNumber).Take(currentTables.Count - 8);
            db.CafeTables.RemoveRange(toRemove);
        }
        db.SaveChanges();
    }

    // ═══ ORDER ID SANITIZATION (COPY-SWAP METHOD) ═══
    try
    {
        Console.WriteLine("[DB CLEANUP] Checking for order ID consistency...");
        var allOrders = db.Orders.OrderBy(o => o.CreatedAt).ToList();
        
        if (allOrders.Any() && allOrders.First().Id != 1)
        {
            int offset = allOrders.First().Id - 1;
            Console.WriteLine($"[DB CLEANUP] Shifting IDs with offset: {offset}");
            
            foreach (var order in allOrders)
            {
                var oldId = order.Id;
                var newId = oldId - offset;

                // 1. Copy the order to the new ID using raw SQL
                // Note: We use double quotes for PostgreSQL compatibility
                db.Database.ExecuteSqlRaw($@"
                    INSERT INTO ""Orders"" (""Id"", ""UserId"", ""TableId"", ""CustomerName"", ""TotalAmount"", ""DiscountApplied"", ""Status"", ""Notes"", ""IsWalkIn"", ""CreatedAt"")
                    SELECT {newId}, ""UserId"", ""TableId"", ""CustomerName"", ""TotalAmount"", ""DiscountApplied"", ""Status"", ""Notes"", ""IsWalkIn"", ""CreatedAt""
                    FROM ""Orders"" WHERE ""Id"" = {oldId}");

                // 2. Point existing items to the new ID
                db.Database.ExecuteSqlRaw($@"UPDATE ""OrderItems"" SET ""OrderId"" = {newId} WHERE ""OrderId"" = {oldId}");

                // 3. Delete the old order record
                db.Database.ExecuteSqlRaw($@"DELETE FROM ""Orders"" WHERE ""Id"" = {oldId}");
            }

            // 4. Reset the sequence so next order is max(Id) + 1
            var maxId = allOrders.Max(o => o.Id) - offset;
            db.Database.ExecuteSqlRaw($"SELECT setval(pg_get_serial_sequence('\"Orders\"', 'Id'), {maxId}, true)");
            
            Console.WriteLine($"[DB CLEANUP] SUCCESS: Shifted {allOrders.Count} orders. Max ID is now {maxId}.");
        }
        else if (!allOrders.Any())
        {
            Console.WriteLine("[DB CLEANUP] No orders found. Resetting sequence to 1.");
            db.Database.ExecuteSqlRaw("SELECT setval(pg_get_serial_sequence('\"Orders\"', 'Id'), 1, false)");
        }
        else
        {
            Console.WriteLine("[DB CLEANUP] Order ID is already 1. No shifting needed.");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[DB CLEANUP] ERROR: {ex.Message}");
        if (ex.InnerException != null) Console.WriteLine($"[DB CLEANUP] INNER ERROR: {ex.InnerException.Message}");
    }
}

app.Run();
