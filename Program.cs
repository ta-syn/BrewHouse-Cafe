using Microsoft.EntityFrameworkCore;
using CafeManagement.Data;
using CafeManagement.Models;
using CafeManagement.Services;
using CafeManagement.Models.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.DataProtection;
using CafeManagement.Hubs;
using CafeManagement.Middleware;

DotNetEnv.Env.Load();
var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddMemoryCache();
builder.Services.AddSignalR();

// 🚀 Database Configuration
builder.Services.AddDbContext<CafeDbContext>(options =>
    options.UseInMemoryDatabase("CafeDb"));

// Session Configuration
builder.Services.AddSession(options => {
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddHttpContextAccessor();

// Register Application Services
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<MenuService>();
builder.Services.AddScoped<OrderService>();
builder.Services.AddScoped<InventoryService>();
builder.Services.AddScoped<EmailService>();
builder.Services.AddScoped<DashboardService>();

// 📊 Phase 4: Background Automation
builder.Services.AddHostedService<CafeManagement.BackgroundTasks.DailyReportingService>();



var app = builder.Build();

// 🛡️ Phase 5: Security Hardening
app.UseRateLimiting();

// Configure the HTTP request pipeline.

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapHub<OrderHub>("/orderHub");

// Runtime Database Initialization & Seeding
try 
{
    using (var scope = app.Services.CreateScope()) 
    {
        var services = scope.ServiceProvider;
        var context = services.GetRequiredService<CafeDbContext>();
        Console.WriteLine("🔄 Initializing Database...");
        context.Database.EnsureCreated();
        Console.WriteLine("✅ Database Initialized Successfully.");
    }
}
catch (Exception ex)
{
    Console.WriteLine("❌ Database Initialization Failed!");
    Console.WriteLine(ex.Message);
    if (ex.InnerException != null)
        Console.WriteLine("Inner Exception: " + ex.InnerException.Message);
}

app.Run();
