using Microsoft.EntityFrameworkCore;
using CafeManagement.Data;
using CafeManagement.Models;
using CafeManagement.Models.Enums;
using CafeManagement.Models.ViewModels;

namespace CafeManagement.Services
{
    public class DashboardService {
        private readonly CafeDbContext _context;
        public DashboardService(CafeDbContext context) { _context = context; }

        public async Task<decimal> GetTotalRevenueAsync() =>
            await _context.Orders
                .Where(o => o.Status != OrderStatus.Cancelled)
                .SumAsync(o => o.TotalAmount);

        public async Task<decimal> GetTodayRevenueAsync() {
            var todayLocal = DateTime.UtcNow.AddHours(6).Date;
            var tomorrowLocal = todayLocal.AddDays(1);
            return await _context.Orders
                .Where(o => o.CreatedAt >= todayLocal && o.CreatedAt < tomorrowLocal && o.Status != OrderStatus.Cancelled)
                .SumAsync(o => o.TotalAmount);
        }

        public async Task<int> GetTodayOrdersAsync() {
            var todayLocal = DateTime.UtcNow.AddHours(6).Date;
            var tomorrowLocal = todayLocal.AddDays(1);
            return await _context.Orders
                .CountAsync(o => o.CreatedAt >= todayLocal && o.CreatedAt < tomorrowLocal);
        }

        public async Task<int> GetTotalOrdersAsync() =>
            await _context.Orders.CountAsync();

        public async Task<int> GetTotalMenuItemsAsync() =>
            await _context.MenuItems.CountAsync();

        // 📊 Phase 4: Intelligence & Analytics
        
        public async Task<List<object>> GetStaffPerformanceAsync() {
            var servedOrders = await _context.Orders
                .Include(o => o.CompletedBy)
                .Where(o => o.Status == OrderStatus.Served && o.CompletedByUserId != null)
                .ToListAsync();

            return servedOrders
                .GroupBy(o => o.CompletedBy!.Name)
                .Select(g => new {
                    staffName = g.Key,
                    ordersCompleted = g.Count(),
                    avgMinutes = g.Average(o => (o.CompletedAt!.Value - o.CreatedAt).TotalMinutes)
                })
                .OrderBy(x => x.avgMinutes)
                .Cast<object>()
                .ToList();
        }

        public async Task<List<object>> GetInventoryForecastAsync() {
            var lastWeek = DateTime.UtcNow.AddHours(6).Date.AddDays(-7);
            
            // 🥗 Advanced Logic: Calculate usage from actual orders + recipes
            var pastOrders = await _context.OrderItems
                .Include(oi => oi.MenuItem)
                    .ThenInclude(m => m!.Recipes)
                .Where(oi => oi.Order!.CreatedAt >= lastWeek && oi.Order.Status == OrderStatus.Served)
                .ToListAsync();

            var inventory = await _context.InventoryItems.ToListAsync();

            return inventory.Select(i => {
                // Sum consumption across all orders in last 7 days
                decimal weeklyConsumption = pastOrders
                    .SelectMany(oi => oi.MenuItem!.Recipes)
                    .Where(r => r.InventoryItemId == i.Id)
                    .Sum(r => r.QuantityRequired);

                // Prediction: Usage next week is likely similar + 15% safety buffer
                decimal predictedNeed = weeklyConsumption * 1.15m;
                
                return new {
                    itemName = i.Name,
                    currentStock = i.StockQuantity,
                    predictedNeed = Math.Round(predictedNeed, 2),
                    unit = i.Unit,
                    status = i.StockQuantity < predictedNeed ? "Critical" : (i.StockQuantity < predictedNeed * 1.5m ? "Low" : "Healthy")
                };
            })
            .OrderByDescending(x => x.predictedNeed)
            .Cast<object>()
            .ToList();
        }

        public async Task<List<string>> GetAISmartInsightsAsync() {
            var insights = new List<string>();
            var now = DateTime.UtcNow.AddHours(6);
            
            // 1. Expiry Check (AI Suggestion for Daily Specials)
            var expiringSoon = await _context.InventoryItems
                .Where(i => i.ExpiryDate != null && i.ExpiryDate <= now.AddDays(3))
                .ToListAsync();
            
            foreach (var item in expiringSoon) {
                insights.Add($"💡 AI Suggestion: Your '{item.Name}' is expiring in {Math.Round((item.ExpiryDate!.Value - now).TotalDays)} days. Use it for a 'Daily Special' combo today!");
            }

            // 2. High Demand Prediction (Heatmap Insight)
            var orders = await _context.Orders.Where(o => o.CreatedAt >= now.AddDays(-30)).ToListAsync();
            var busyHour = orders.GroupBy(o => o.CreatedAt.Hour).OrderByDescending(g => g.Count()).FirstOrDefault()?.Key;
            if (busyHour.HasValue) {
                insights.Add($"🔥 Trend Alert: Your peak hour is {busyHour:00}:00. We recommend having 1 extra staff member active during this window.");
            }

            // 3. Category Shift
            var lastMonth = now.AddDays(-30);
            var topCategory = await _context.OrderItems
                .Where(oi => oi.Order!.CreatedAt >= lastMonth)
                .GroupBy(oi => oi.MenuItem!.Category)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key)
                .FirstOrDefaultAsync();

            if (!string.IsNullOrEmpty(topCategory)) {
                insights.Add($"🚀 Category Insight: '{topCategory}' is your fastest-growing category this month. Expand this section of your menu!");
            }

            return insights;
        }

        public async Task<object> GetFinancialReportAsync() {
            var now = DateTime.UtcNow.AddHours(6);
            var startOfMonth = new DateTime(now.Year, now.Month, 1);
            
            decimal grossRevenue = await _context.Orders
                .Where(o => o.CreatedAt >= startOfMonth && o.Status == OrderStatus.Served)
                .SumAsync(o => o.TotalAmount);
            
            // COGS: In real-world, this would sum UnitCost from Inventory records
            decimal cogs = grossRevenue * 0.35m; // Assumed 35% margin for this demo
            decimal grossProfit = grossRevenue - cogs;
            
            return new {
                revenue = grossRevenue,
                expenses = cogs,
                profit = grossProfit,
                margin = grossRevenue > 0 ? (grossProfit / grossRevenue) * 100 : 0
            };
        }

        public async Task<List<object>> GetSalesTrendAsync(int days = 7) {
            var startDate = DateTime.UtcNow.AddHours(6).Date.AddDays(-(days - 1));
            var orders = await _context.Orders
                .Where(o => o.CreatedAt >= startDate && o.Status != OrderStatus.Cancelled)
                .ToListAsync();

            return Enumerable.Range(0, days)
                .Select(offset => startDate.AddDays(offset))
                .Select(date => new {
                    date = date.ToString("MMM dd"),
                    revenue = orders.Where(o => o.CreatedAt.Date == date).Sum(o => o.TotalAmount),
                    orders = orders.Count(o => o.CreatedAt.Date == date)
                })
                .Cast<object>()
                .ToList();
        }

        public async Task<List<object>> GetHourlyTrafficAsync() {
            var orders = await _context.Orders
                .Where(o => o.Status != OrderStatus.Cancelled)
                .ToListAsync();

            return orders
                .GroupBy(o => o.CreatedAt.Hour)
                .Select(g => new {
                    hour = $"{g.Key:00}:00",
                    count = g.Count()
                })
                .OrderBy(x => x.hour)
                .Cast<object>()
                .ToList();
        }

        public async Task<List<object>> GetCategorySalesAsync() {
            return await _context.OrderItems
                .Include(oi => oi.MenuItem)
                .GroupBy(oi => oi.MenuItem!.Category)
                .Select(g => new {
                    category = g.Key,
                    count = g.Sum(x => x.Quantity)
                })
                .OrderByDescending(x => x.count)
                .Cast<object>()
                .ToListAsync();
        }

        public async Task<List<PopularItemViewModel>> GetPopularItemsAsync(int top = 5) {
            try {
                return await _context.OrderItems
                    .Include(oi => oi.MenuItem)
                    .GroupBy(oi => new { oi.MenuItemId, oi.ItemName, oi.MenuItem!.ImageUrl, oi.MenuItem.ImageEmoji })
                    .Select(g => new PopularItemViewModel {
                        MenuItemId = g.Key.MenuItemId,
                        ItemName = g.Key.ItemName,
                        ImageUrl = g.Key.ImageUrl,
                        ImageEmoji = g.Key.ImageEmoji ?? "☕",
                        TotalOrdered = g.Sum(x => x.Quantity),
                        TotalRevenue = g.Sum(x => x.Quantity * x.UnitPrice)
                    })
                    .OrderByDescending(x => x.TotalOrdered)
                    .Take(top)
                    .ToListAsync();
            } catch (Exception) { return new List<PopularItemViewModel>(); }
        }

        public async Task<List<Order>> GetRecentOrdersAsync(int count = 10) =>
            await _context.Orders
                .Include(o => o.User)
                .Include(o => o.Table)
                .OrderByDescending(o => o.CreatedAt)
                .Take(count)
                .ToListAsync();
    }
}
