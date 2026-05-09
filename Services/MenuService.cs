using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using CafeManagement.Data;
using CafeManagement.Models;
using CafeManagement.Exceptions;

namespace CafeManagement.Services
{
    // ⚡ Phase 5.1: High Performance Menu Service with Caching
    public class MenuService
    {
        private readonly CafeDbContext _context;
        private readonly IMemoryCache _cache;
        private const string MenuCacheKey = "FullMenuCache";

        public MenuService(CafeDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public async Task<List<MenuItem>> GetAllItemsAsync()
        {
            if (!_cache.TryGetValue(MenuCacheKey, out List<MenuItem>? items))
            {
                items = await _context.MenuItems
                    .Include(m => m.Recipes)
                        .ThenInclude(r => r.InventoryItem)
                    .OrderBy(m => m.Category)
                    .ToListAsync();

                _cache.Set(MenuCacheKey, items, TimeSpan.FromMinutes(10));
            }
            return items ?? new List<MenuItem>();
        }

        // 🟢 Added for HomeController
        public async Task<List<MenuItem>> GetAvailableItemsAsync()
        {
            var all = await GetAllItemsAsync();
            return all.Where(m => m.IsAvailable).ToList();
        }

        // 🟢 Added for HomeController
        public async Task<List<MenuItem>> GetByCategoryAsync(string category)
        {
            var all = await GetAllItemsAsync();
            return all.Where(m => m.Category == category && m.IsAvailable).ToList();
        }

        public async Task AddItemAsync(MenuItem item)
        {
            _context.MenuItems.Add(item);
            await _context.SaveChangesAsync();
            _cache.Remove(MenuCacheKey);
        }

        public async Task UpdateItemAsync(MenuItem item)
        {
            var existing = await _context.MenuItems.FindAsync(item.Id);
            if (existing == null) throw new ItemNotFoundException("Item not found.");
            
            _context.Entry(existing).CurrentValues.SetValues(item);
            await _context.SaveChangesAsync();
            _cache.Remove(MenuCacheKey);
        }

        public async Task DeleteItemAsync(int id)
        {
            var item = await _context.MenuItems.FindAsync(id);
            if (item == null) throw new ItemNotFoundException("Item not found.");
            _context.MenuItems.Remove(item);
            await _context.SaveChangesAsync();
            _cache.Remove(MenuCacheKey);
        }

        public async Task<MenuItem?> GetByIdAsync(int id) =>
            await _context.MenuItems
                .Include(m => m.Recipes)
                .FirstOrDefaultAsync(m => m.Id == id);

        public async Task<List<MenuItem>> SearchAsync(string query) =>
            await _context.MenuItems
                .Where(m => m.Name.Contains(query) || m.Category.Contains(query))
                .ToListAsync();

        public async Task ToggleAvailabilityAsync(int id)
        {
            var item = await _context.MenuItems.FindAsync(id);
            if (item != null)
            {
                item.IsAvailable = !item.IsAvailable;
                await _context.SaveChangesAsync();
                _cache.Remove(MenuCacheKey);
            }
        }

        public async Task DeleteCategoryAsync(string category)
        {
            var items = await _context.MenuItems.Where(m => m.Category == category).ToListAsync();
            _context.MenuItems.RemoveRange(items);
            await _context.SaveChangesAsync();
            _cache.Remove(MenuCacheKey);
        }
    }
}
