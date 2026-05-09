using Microsoft.AspNetCore.Mvc;
using CafeManagement.Services;
using CafeManagement.Models.Enums;
using CafeManagement.Models;

namespace CafeManagement.Controllers
{
    public class HomeController : Controller {
        private readonly MenuService _menuService;

        public HomeController(MenuService menuService) {
            _menuService = menuService;
        }

        public async Task<IActionResult> Index() {
            var items = await _menuService.GetAvailableItemsAsync();
            return View(items.Take(6).ToList());
        }

        public async Task<IActionResult> Menu(string? category, string? search, string? tid) {
            // 📱 Phase 3: QR Ordering Detection
            if (!string.IsNullOrEmpty(tid)) {
                try {
                    // Decrypt tid (Base64 decode)
                    byte[] data = Convert.FromBase64String(tid);
                    string decodedString = System.Text.Encoding.UTF8.GetString(data);
                    if (decodedString.StartsWith("table_")) {
                        string idStr = decodedString.Replace("table_", "");
                        if (int.TryParse(idStr, out int tableId)) {
                            HttpContext.Session.SetInt32("AutoTableId", tableId);
                            TempData["Info"] = $"Welcome! You are ordering from Table {tableId}.";
                        }
                    }
                } catch { /* Invalid QR link */ }
            }

            IEnumerable<MenuItem> items;

            if (!string.IsNullOrEmpty(search))
                items = await _menuService.SearchAsync(search);
            else if (string.IsNullOrEmpty(category))
                items = await _menuService.GetAvailableItemsAsync();
            else
                items = await _menuService.GetByCategoryAsync(category);
            
            var allItems = await _menuService.GetAllItemsAsync();
            ViewBag.Categories = allItems.Select(i => i.Category).Distinct().ToList();
            
            ViewBag.SelectedCategory = category;
            ViewBag.SearchQuery = search;
            return View(items.ToList());
        }

        public async Task<IActionResult> ItemDetail(int id) {
            var item = await _menuService.GetByIdAsync(id);
            if (item == null) return NotFound();
            return View(item);
        }

        public IActionResult About() => View();

        public IActionResult PageNotFound() => View("NotFound");

        public IActionResult Error() => View();
    }
}
