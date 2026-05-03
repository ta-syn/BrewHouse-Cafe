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

        public async Task<IActionResult> Menu(string? category, string? search) {
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
