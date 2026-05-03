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

        // FIX: returns typed PopularItemViewModel, not object
        public async Task<List<PopularItemViewModel>> GetPopularItemsAsync(int top = 5) {
            // ═══ OOP CONCEPT: EXCEPTION HANDLING ═══
            try {
                return await _context.OrderItems
                    .Include(oi => oi.MenuItem)
                    .GroupBy(oi => new { oi.MenuItemId, oi.ItemName, oi.MenuItem.ImageUrl, oi.MenuItem.ImageEmoji })
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
