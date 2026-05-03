using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CafeManagement.Data;
using CafeManagement.Models;
using CafeManagement.Models.Enums;
using CafeManagement.Models.ViewModels;
using CafeManagement.Services;
using CafeManagement.Filters;
using CafeManagement.Exceptions;

namespace CafeManagement.Controllers
{
    [SessionAuthorize("Admin")]
    public class AdminController : Controller {
        private readonly MenuService _menuService;
        private readonly OrderService _orderService;
        private readonly DashboardService _dashboardService;
        private readonly AuthService _authService;
        private readonly CafeDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public AdminController(
            MenuService menuService,
            OrderService orderService,
            DashboardService dashboardService,
            AuthService authService,
            CafeDbContext context,
            IWebHostEnvironment hostEnvironment)
        {
            _menuService = menuService;
            _orderService = orderService;
            _dashboardService = dashboardService;
            _authService = authService;
            _context = context;
            _hostEnvironment = hostEnvironment;
        }

        public async Task<IActionResult> Dashboard() {
            try {
                var vm = new AdminDashboardViewModel {
                    TotalRevenue = await _dashboardService.GetTotalRevenueAsync(),
                    TodayRevenue = await _dashboardService.GetTodayRevenueAsync(),
                    TotalOrders = await _dashboardService.GetTotalOrdersAsync(),
                    TodayOrders = await _dashboardService.GetTodayOrdersAsync(),
                    TotalItems = await _dashboardService.GetTotalMenuItemsAsync(),
                    PopularItems = await _dashboardService.GetPopularItemsAsync(5),
                    RecentOrders = await _dashboardService.GetRecentOrdersAsync(10)
                };
                return View(vm);
            } catch (Exception ex) {
                Console.WriteLine($"[{DateTime.Now}] Dashboard Error: {ex.Message}");
                return RedirectToAction("Error", "Home");
            }
        }

        public async Task<IActionResult> MenuList(string? search) {
            try {
                var items = string.IsNullOrEmpty(search)
                    ? await _menuService.GetAllItemsAsync()
                    : await _menuService.SearchAsync(search);
                return View(items);
            } catch (Exception ex) {
                TempData["Error"] = ex.Message;
                return View(new List<MenuItem>());
            }
        }

        [HttpGet] public async Task<IActionResult> AddItem() {
            var items = await _menuService.GetAllItemsAsync();
            ViewBag.Categories = items.Select(i => i.Category).Distinct().ToList();
            ViewBag.CategoryColors = items.Where(i => !string.IsNullOrEmpty(i.CategoryColor))
                .GroupBy(i => i.Category)
                .ToDictionary(g => g.Key, g => g.First().CategoryColor);
            return View(new AddItemViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> AddItem(AddItemViewModel vm) {
            if (!ModelState.IsValid) {
                var items = await _menuService.GetAllItemsAsync();
                ViewBag.Categories = items.Select(i => i.Category).Distinct().ToList();
                ViewBag.CategoryColors = items.Where(i => !string.IsNullOrEmpty(i.CategoryColor))
                    .GroupBy(i => i.Category)
                    .ToDictionary(g => g.Key, g => g.First().CategoryColor);
                return View(vm);
            }
            try {
                string finalCategory = vm.Category == "Add New" ? vm.NewCategory ?? "General" : vm.Category;
                string finalType = vm.ItemType == "Add New" ? vm.NewItemType ?? "Snack" : vm.ItemType;
                
                string imageUrl = "";
                if (vm.ImageFile != null) {
                    string uploads = Path.Combine(_hostEnvironment.WebRootPath, "images", "menu");
                    if (!Directory.Exists(uploads)) Directory.CreateDirectory(uploads);
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(vm.ImageFile.FileName);
                    string filePath = Path.Combine(uploads, fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create)) {
                        await vm.ImageFile.CopyToAsync(stream);
                    }
                    imageUrl = "/images/menu/" + fileName;
                }

                MenuItem item = finalType switch {
                    "Beverage" => new Beverage {
                        Name = vm.Name, Price = vm.Price, Description = vm.Description,
                        Category = finalCategory, CategoryColor = vm.CategoryColor, ImageEmoji = vm.ImageEmoji, ImageUrl = imageUrl,
                        IsAvailable = vm.IsAvailable, IsHot = vm.IsHot, Size = vm.Size
                    },
                    "Food" => new Food {
                        Name = vm.Name, Price = vm.Price, Description = vm.Description,
                        Category = finalCategory, CategoryColor = vm.CategoryColor, ImageEmoji = vm.ImageEmoji, ImageUrl = imageUrl,
                        IsAvailable = vm.IsAvailable,
                        PreparationTimeMinutes = vm.PreparationTimeMinutes,
                        IsVegetarian = vm.IsVegetarian
                    },
                    "Dessert" => new Dessert {
                        Name = vm.Name, Price = vm.Price, Description = vm.Description,
                        Category = finalCategory, CategoryColor = vm.CategoryColor, ImageEmoji = vm.ImageEmoji, ImageUrl = imageUrl,
                        IsAvailable = vm.IsAvailable, Allergens = vm.Allergens,
                        IsSeasonalItem = vm.IsSeasonalItem
                    },
                    "Snack" => new Snack {
                        Name = vm.Name, Price = vm.Price, Description = vm.Description,
                        Category = finalCategory, CategoryColor = vm.CategoryColor, ImageEmoji = vm.ImageEmoji, ImageUrl = imageUrl,
                        IsAvailable = vm.IsAvailable
                    },
                    _ => new Snack { // Default for "Add New" Item Type
                        Name = vm.Name, Price = vm.Price, Description = vm.Description,
                        Category = finalCategory, CategoryColor = vm.CategoryColor, ImageEmoji = vm.ImageEmoji, ImageUrl = imageUrl,
                        IsAvailable = vm.IsAvailable
                    }
                };
                await _menuService.AddItemAsync(item);
                TempData["Success"] = "Item added successfully!";
                return RedirectToAction("MenuList");
            } catch (Exception ex) {
                TempData["Error"] = ex.Message;
                return View(vm);
            }
        }

        [HttpGet] public async Task<IActionResult> EditItem(int id) {
            try {
                var item = await _menuService.GetByIdAsync(id);
                if (item == null) throw new ItemNotFoundException("Item not found.");

                var allItems = await _menuService.GetAllItemsAsync();
                ViewBag.Categories = allItems.Select(i => i.Category).Distinct().ToList();
                ViewBag.CategoryColors = allItems.Where(i => !string.IsNullOrEmpty(i.CategoryColor))
                    .GroupBy(i => i.Category)
                    .ToDictionary(g => g.Key, g => g.First().CategoryColor);

                // Map to ViewModel
                var vm = new AddItemViewModel {
                    ItemType = item.GetType().Name,
                    Name = item.Name, Price = item.Price,
                    Description = item.Description, ImageEmoji = item.ImageEmoji,
                    IsAvailable = item.IsAvailable,
                    Category = item.Category,
                    CategoryColor = item.CategoryColor
                };
                if (item is Beverage b) { vm.IsHot = b.IsHot; vm.Size = b.Size; }
                else if (item is Food f) { vm.PreparationTimeMinutes = f.PreparationTimeMinutes; vm.IsVegetarian = f.IsVegetarian; }
                else if (item is Dessert d) { vm.Allergens = d.Allergens; vm.IsSeasonalItem = d.IsSeasonalItem; }
                ViewBag.ItemId = id;
                return View(vm);
            } catch (Exception ex) {
                TempData["Error"] = ex.Message;
                return RedirectToAction("MenuList");
            }
        }

        [HttpPost]
        public async Task<IActionResult> EditItem(int id, AddItemViewModel vm) {
            if (!ModelState.IsValid) { ViewBag.ItemId = id; return View(vm); }
            try {
                string finalCategory = vm.Category == "Add New" ? vm.NewCategory ?? "General" : vm.Category;
                string finalType = vm.ItemType == "Add New" ? vm.NewItemType ?? "Snack" : vm.ItemType;
                
                string imageUrl = "";
                if (vm.ImageFile != null) {
                    string uploads = Path.Combine(_hostEnvironment.WebRootPath, "images", "menu");
                    if (!Directory.Exists(uploads)) Directory.CreateDirectory(uploads);
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(vm.ImageFile.FileName);
                    using (var stream = new FileStream(Path.Combine(uploads, fileName), FileMode.Create)) {
                        await vm.ImageFile.CopyToAsync(stream);
                    }
                    imageUrl = "/images/menu/" + fileName;
                }

                MenuItem item = finalType switch {
                    "Beverage" => new Beverage { Id = id, Name = vm.Name, Price = vm.Price,
                        Description = vm.Description, Category = finalCategory, CategoryColor = vm.CategoryColor,
                        ImageEmoji = vm.ImageEmoji, ImageUrl = imageUrl, IsAvailable = vm.IsAvailable,
                        IsHot = vm.IsHot, Size = vm.Size },
                    "Food" => new Food { Id = id, Name = vm.Name, Price = vm.Price,
                        Description = vm.Description, Category = finalCategory, CategoryColor = vm.CategoryColor,
                        ImageEmoji = vm.ImageEmoji, ImageUrl = imageUrl, IsAvailable = vm.IsAvailable,
                        PreparationTimeMinutes = vm.PreparationTimeMinutes, IsVegetarian = vm.IsVegetarian },
                    "Dessert" => new Dessert { Id = id, Name = vm.Name, Price = vm.Price,
                        Description = vm.Description, Category = finalCategory, CategoryColor = vm.CategoryColor,
                        ImageEmoji = vm.ImageEmoji, ImageUrl = imageUrl, IsAvailable = vm.IsAvailable,
                        Allergens = vm.Allergens, IsSeasonalItem = vm.IsSeasonalItem },
                    "Snack" => new Snack { Id = id, Name = vm.Name, Price = vm.Price,
                        Description = vm.Description, Category = finalCategory, CategoryColor = vm.CategoryColor,
                        ImageEmoji = vm.ImageEmoji, ImageUrl = imageUrl, IsAvailable = vm.IsAvailable },
                    "Special" => new Special { Id = id, Name = vm.Name, Price = vm.Price,
                        Description = vm.Description, Category = finalCategory, CategoryColor = vm.CategoryColor,
                        ImageEmoji = vm.ImageEmoji, ImageUrl = imageUrl, IsAvailable = vm.IsAvailable },
                    _ => new Snack { Id = id, Name = vm.Name, Price = vm.Price,
                        Description = vm.Description, Category = finalCategory, CategoryColor = vm.CategoryColor,
                        ImageEmoji = vm.ImageEmoji, ImageUrl = imageUrl, IsAvailable = vm.IsAvailable }
                };
                await _menuService.UpdateItemAsync(item);
                TempData["Success"] = "Item updated!";
                return RedirectToAction("MenuList");
            } catch (Exception ex) {
                TempData["Error"] = ex.Message;
                return View(vm);
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteItem(int id) {
            try {
                await _menuService.DeleteItemAsync(id);
                TempData["Success"] = "Item deleted.";
            } catch (Exception ex) { TempData["Error"] = ex.Message; }
            return RedirectToAction("MenuList");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCategory(string category) {
            if (string.IsNullOrEmpty(category)) return BadRequest();
            try {
                await _menuService.DeleteCategoryAsync(category);
                TempData["Success"] = $"Category '{category}' and all its items deleted.";
            } catch (Exception ex) {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction(nameof(MenuList));
        }

        [HttpPost]
        public async Task<IActionResult> ToggleAvailability(int id) {
            try { await _menuService.ToggleAvailabilityAsync(id); }
            catch (Exception ex) { TempData["Error"] = ex.Message; }
            return RedirectToAction("MenuList");
        }

        public async Task<IActionResult> AllOrders(string? status) {
            try {
                var orders = await _orderService.GetAllOrdersAsync();
                if (!string.IsNullOrEmpty(status) &&
                    Enum.TryParse<OrderStatus>(status, out var s))
                    orders = orders.Where(o => o.Status == s).ToList();
                return View(orders);
            } catch (Exception ex) {
                TempData["Error"] = ex.Message;
                return View(new List<Order>());
            }
        }

        public async Task<IActionResult> OrderDetail(int id) {
            try {
                var order = await _orderService.GetOrderByIdAsync(id);
                if (order == null) throw new ItemNotFoundException("Order not found.");
                return View(order);
            } catch (Exception ex) {
                TempData["Error"] = ex.Message;
                return RedirectToAction("AllOrders");
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateOrderStatus(int orderId, OrderStatus status) {
            try { await _orderService.UpdateStatusAsync(orderId, status); TempData["Success"] = "Status updated."; }
            catch (Exception ex) { TempData["Error"] = ex.Message; }
            return RedirectToAction("OrderDetail", new { id = orderId });
        }

        public async Task<IActionResult> StaffList() =>
            View(await _authService.GetAllStaffAsync());

        [HttpGet] public IActionResult AddStaff() => View(new RegisterViewModel());

        [HttpPost]
        public async Task<IActionResult> AddStaff(RegisterViewModel vm) {
            if (!ModelState.IsValid) return View(vm);
            try {
                await _authService.RegisterAsync(vm.Name, vm.Email, vm.Password);
                // Set role to Staff
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == vm.Email);
                if (user != null) { user.Role = UserRole.Staff; await _context.SaveChangesAsync(); }
                TempData["Success"] = "Staff added!";
                return RedirectToAction("StaffList");
            } catch (DuplicateEmailException ex) {
                ModelState.AddModelError("Email", ex.Message);
                return View(vm);
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteStaff(int id) {
            try { await _authService.DeleteUserAsync(id); TempData["Success"] = "Staff removed."; }
            catch (Exception ex) { TempData["Error"] = ex.Message; }
            return RedirectToAction("StaffList");
        }

        public async Task<IActionResult> TableList() =>
            View(await _context.CafeTables.ToListAsync());

        [HttpPost]
        public async Task<IActionResult> UpdateTableStatus(int id, TableStatus status) {
            try {
                var table = await _context.CafeTables.FindAsync(id);
                if (table == null) throw new ItemNotFoundException("Table not found.");
                table.Status = status;
                await _context.SaveChangesAsync();
                TempData["Success"] = "Table status updated.";
            } catch (Exception ex) { TempData["Error"] = ex.Message; }
            return RedirectToAction("TableList");
        }
    }
}
