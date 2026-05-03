using Microsoft.EntityFrameworkCore;
using CafeManagement.Data;
using CafeManagement.Models;
using CafeManagement.Models.Enums;
using CafeManagement.Exceptions;

namespace CafeManagement.Services
{
    public class MenuService {
        private readonly CafeDbContext _context;
        public MenuService(CafeDbContext context) { _context = context; }

        public async Task<List<MenuItem>> GetAllItemsAsync() =>
            await _context.MenuItems.Where(m => true).ToListAsync();

        public async Task<List<MenuItem>> GetAvailableItemsAsync() =>
            await _context.MenuItems.Where(m => m.IsAvailable).ToListAsync();

        public async Task<List<MenuItem>> GetByCategoryAsync(string category) =>
            await _context.MenuItems.Where(m => m.Category == category && m.IsAvailable).ToListAsync();

        public async Task<MenuItem?> GetByIdAsync(int id) =>
            await _context.MenuItems.FindAsync(id);

        public async Task<List<MenuItem>> SearchAsync(string query) {
            var q = query.ToLower();
            return await _context.MenuItems
                .Where(m => m.Name.ToLower().Contains(q) || m.Description.ToLower().Contains(q))
                .ToListAsync();
        }

        public async Task AddItemAsync(MenuItem item) {
            // ═══ OOP CONCEPT: EXCEPTION HANDLING ═══
            try {
                item.CreatedAt = DateTime.UtcNow;
                _context.MenuItems.Add(item);
                await _context.SaveChangesAsync();
                
                // Sync color for all items in this category
                if (!string.IsNullOrEmpty(item.CategoryColor)) {
                    await SyncCategoryColorAsync(item.Category, item.CategoryColor);
                }
            } catch (Exception ex) {
                Console.WriteLine($"[{DateTime.Now}] AddItem Error: {ex.Message}");
                throw new CafeException("Failed to add menu item.");
            }
        }

        public async Task UpdateItemAsync(MenuItem updatedItem) {
            // ═══ OOP CONCEPT: EXCEPTION HANDLING ═══
            try {
                // FIX: TPH — update only scalar properties, don't change type
                var existing = await _context.MenuItems.FindAsync(updatedItem.Id);
                if (existing == null) throw new ItemNotFoundException("Menu item not found.");
                
                bool colorChanged = existing.CategoryColor != updatedItem.CategoryColor;
                
                existing.Name = updatedItem.Name;
                existing.Price = updatedItem.Price;
                existing.Description = updatedItem.Description;
                existing.Category = updatedItem.Category;
                existing.CategoryColor = updatedItem.CategoryColor;
                existing.IsAvailable = updatedItem.IsAvailable;
                existing.ImageEmoji = updatedItem.ImageEmoji;
                if (!string.IsNullOrEmpty(updatedItem.ImageUrl))
                    existing.ImageUrl = updatedItem.ImageUrl;

                // Update subclass-specific properties
                if (existing is Beverage bev && updatedItem is Beverage updBev) {
                    bev.IsHot = updBev.IsHot;
                    bev.Size = updBev.Size;
                } else if (existing is Food food && updatedItem is Food updFood) {
                    food.PreparationTimeMinutes = updFood.PreparationTimeMinutes;
                    food.IsVegetarian = updFood.IsVegetarian;
                } else if (existing is Dessert des && updatedItem is Dessert updDes) {
                    des.Allergens = updDes.Allergens;
                    des.IsSeasonalItem = updDes.IsSeasonalItem;
                }
                
                await _context.SaveChangesAsync();

                // If color was changed, sync it across all items in this category
                if (colorChanged && !string.IsNullOrEmpty(existing.CategoryColor)) {
                    await SyncCategoryColorAsync(existing.Category, existing.CategoryColor);
                }
            } catch (CafeException) { throw; }
            catch (Exception ex) {
                Console.WriteLine($"[{DateTime.Now}] UpdateItem Error: {ex.Message}");
                throw new CafeException("Failed to update menu item.");
            }
        }

        private async Task SyncCategoryColorAsync(string category, string color) {
            var items = await _context.MenuItems
                .Where(m => m.Category == category && m.CategoryColor != color)
                .ToListAsync();
            
            if (items.Any()) {
                foreach (var item in items) {
                    item.CategoryColor = color;
                }
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteItemAsync(int id) {
            // ═══ OOP CONCEPT: EXCEPTION HANDLING ═══
            try {
                var item = await _context.MenuItems.FindAsync(id);
                if (item == null) throw new ItemNotFoundException("Menu item not found.");
                _context.MenuItems.Remove(item);
                await _context.SaveChangesAsync();
            } catch (CafeException) { throw; }
            catch (Exception ex) {
                Console.WriteLine($"[{DateTime.Now}] DeleteItem Error: {ex.Message}");
                throw;
            }
        }

        public async Task DeleteCategoryAsync(string category) {
            try {
                var items = await _context.MenuItems.Where(m => m.Category == category).ToListAsync();
                if (items.Any()) {
                    _context.MenuItems.RemoveRange(items);
                    await _context.SaveChangesAsync();
                }
            } catch (Exception ex) {
                Console.WriteLine($"[{DateTime.Now}] DeleteCategory Error: {ex.Message}");
                throw new CafeException($"Failed to delete category {category}.");
            }
        }

        public async Task ToggleAvailabilityAsync(int id) {
            var item = await _context.MenuItems.FindAsync(id);
            if (item == null) throw new ItemNotFoundException("Item not found.");
            item.IsAvailable = !item.IsAvailable;
            await _context.SaveChangesAsync();
        }
    }
}
