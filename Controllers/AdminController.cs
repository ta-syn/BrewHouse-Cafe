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
        private readonly InventoryService _inventoryService;
        private readonly CafeDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public AdminController(
            MenuService menuService,
            OrderService orderService,
            DashboardService dashboardService,
            AuthService authService,
            InventoryService inventoryService,
            CafeDbContext context,
            IWebHostEnvironment hostEnvironment)
        {
            _menuService = menuService;
            _orderService = orderService;
            _dashboardService = dashboardService;
            _authService = authService;
            _inventoryService = inventoryService;
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
                    TotalCustomers = await _context.Users.CountAsync(u => u.Role == UserRole.Customer),
                    PopularItems = await _dashboardService.GetPopularItemsAsync(5),
                    RecentOrders = await _dashboardService.GetRecentOrdersAsync(10)
                };
                return View(vm);
            } catch (Exception ex) {
                Console.WriteLine($"[{DateTime.Now}] Dashboard Error: {ex.Message}");
                return RedirectToAction("Error", "Home");
            }
        }

        // 📊 Phase 4: AI & Data Insights
        public async Task<IActionResult> Analytics() {
            ViewBag.SalesTrend = await _dashboardService.GetSalesTrendAsync(7);
            ViewBag.HourlyTraffic = await _dashboardService.GetHourlyTrafficAsync();
            ViewBag.CategorySales = await _dashboardService.GetCategorySalesAsync();
            
            // Real Analytics from Phase 4 Implementation
            ViewBag.StaffPerformance = await _dashboardService.GetStaffPerformanceAsync();
            ViewBag.InventoryForecast = await _dashboardService.GetInventoryForecastAsync();
            ViewBag.FinancialReport = await _dashboardService.GetFinancialReportAsync();
            ViewBag.AIInsights = await _dashboardService.GetAISmartInsightsAsync();

            return View();
        }


        // 🛰️ Phase 2: Inventory Management
        public async Task<IActionResult> Inventory() {
            var items = await _inventoryService.GetAllInventoryAsync();
            return View(items);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateInventory(int id, decimal quantity) {
            try {
                await _inventoryService.UpdateStockAsync(id, quantity);
                TempData["Success"] = "Inventory updated!";
            } catch (Exception ex) {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction("Inventory");
        }

        [HttpPost]
        public async Task<IActionResult> RecordWastage(int id, decimal quantity, string reason) {
            try {
                await _inventoryService.RecordWastageAsync(id, quantity, reason);
                TempData["Success"] = "Wastage recorded successfully.";
            } catch (Exception ex) {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction("Inventory");
        }

        [HttpGet]
        public async Task<IActionResult> GetWastageHistory() {
            var history = await _context.WastageLogs
                .Include(w => w.InventoryItem)
                .OrderByDescending(w => w.RecordedAt)
                .Take(20)
                .Select(w => new {
                    itemName = w.InventoryItem != null ? w.InventoryItem.Name : "Unknown",
                    quantity = w.Quantity,
                    reason = w.Reason,
                    date = w.RecordedAt
                })
                .ToListAsync();
            return Json(history);
        }

        // 🛰️ Phase 2: Recipe Management
        public async Task<IActionResult> ManageRecipe(int id) {
            var item = await _context.MenuItems
                .Include(m => m.Recipes)
                .ThenInclude(r => r.InventoryItem)
                .FirstOrDefaultAsync(m => m.Id == id);
            
            if (item == null) return NotFound();
            
            ViewBag.InventoryItems = await _inventoryService.GetAllInventoryAsync();
            return View(item);
        }

        [HttpPost]
        public async Task<IActionResult> AddRecipeItem(int menuItemId, int inventoryItemId, decimal quantity) {
            try {
                var recipe = new RecipeItem {
                    MenuItemId = menuItemId,
                    InventoryItemId = inventoryItemId,
                    QuantityRequired = quantity
                };
                _context.RecipeItems.Add(recipe);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Ingredient added to recipe.";
            } catch (Exception ex) { TempData["Error"] = ex.Message; }
            return RedirectToAction("ManageRecipe", new { id = menuItemId });
        }

        [HttpPost]
        public async Task<IActionResult> RemoveRecipeItem(int id) {
            var recipe = await _context.RecipeItems.FindAsync(id);
            if (recipe == null) return NotFound();
            int menuItemId = recipe.MenuItemId;
            _context.RecipeItems.Remove(recipe);
            await _context.SaveChangesAsync();
            return RedirectToAction("ManageRecipe", new { id = menuItemId });
        }

        // 🛰️ Phase 2: Notification Management
        [HttpGet]
        public async Task<IActionResult> GetNotifications() {
            var notifications = await _context.Notifications
                .OrderByDescending(n => n.CreatedAt)
                .Take(10)
                .ToListAsync();
            
            var unreadCount = await _context.Notifications.CountAsync(n => !n.IsRead);
            
            return Json(new { notifications, unreadCount });
        }

        [HttpPost]
        public async Task<IActionResult> MarkNotificationsAsRead() {
            var unread = await _context.Notifications.Where(n => !n.IsRead).ToListAsync();
            unread.ForEach(n => n.IsRead = true);
            await _context.SaveChangesAsync();
            return Ok();
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

                string categoryColor = !string.IsNullOrEmpty(vm.CategoryColor) && vm.CategoryColor != "#6c757d" 
                    ? vm.CategoryColor 
                    : GetCategoryColor(finalCategory);

                MenuItem item = finalType switch {
                    "Beverage" => new Beverage {
                        Name = vm.Name, Price = vm.Price, Description = vm.Description,
                        Category = finalCategory, CategoryColor = categoryColor, ImageEmoji = vm.ImageEmoji, ImageUrl = imageUrl,
                        IsAvailable = vm.IsAvailable, IsHot = vm.IsHot, Size = vm.Size, OutletId = 1
                    },
                    "Food" => new Food {
                        Name = vm.Name, Price = vm.Price, Description = vm.Description,
                        Category = finalCategory, CategoryColor = categoryColor, ImageEmoji = vm.ImageEmoji, ImageUrl = imageUrl,
                        IsAvailable = vm.IsAvailable,
                        PreparationTimeMinutes = vm.PreparationTimeMinutes,
                        IsVegetarian = vm.IsVegetarian, OutletId = 1
                    },
                    "Dessert" => new Dessert {
                        Name = vm.Name, Price = vm.Price, Description = vm.Description,
                        Category = finalCategory, CategoryColor = categoryColor, ImageEmoji = vm.ImageEmoji, ImageUrl = imageUrl,
                        IsAvailable = vm.IsAvailable, Allergens = vm.Allergens,
                        IsSeasonalItem = vm.IsSeasonalItem, OutletId = 1
                    },
                    "Snack" => new Snack {
                        Name = vm.Name, Price = vm.Price, Description = vm.Description,
                        Category = finalCategory, CategoryColor = categoryColor, ImageEmoji = vm.ImageEmoji, ImageUrl = imageUrl,
                        IsAvailable = vm.IsAvailable, OutletId = 1
                    },
                    _ => new Snack {
                        Name = vm.Name, Price = vm.Price, Description = vm.Description,
                        Category = finalCategory, CategoryColor = categoryColor, ImageEmoji = vm.ImageEmoji, ImageUrl = imageUrl,
                        IsAvailable = vm.IsAvailable, OutletId = 1
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
                
                var existingItem = await _menuService.GetByIdAsync(id);
                if (existingItem == null) throw new ItemNotFoundException("Item not found.");

                // Keep existing image by default
                string imageUrl = existingItem.ImageUrl; 
                if (vm.ImageFile != null) {
                    string uploads = Path.Combine(_hostEnvironment.WebRootPath, "images", "menu");
                    if (!Directory.Exists(uploads)) Directory.CreateDirectory(uploads);
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(vm.ImageFile.FileName);
                    using (var stream = new FileStream(Path.Combine(uploads, fileName), FileMode.Create)) {
                        await vm.ImageFile.CopyToAsync(stream);
                    }
                    imageUrl = "/images/menu/" + fileName;
                }

                // If color is changed, update ALL items in this category to keep it "Category Wise"
                string categoryColor = vm.CategoryColor ?? existingItem.CategoryColor;
                if (categoryColor != existingItem.CategoryColor) {
                    var categoryItems = await _context.MenuItems.Where(m => m.Category == finalCategory).ToListAsync();
                    foreach(var ci in categoryItems) {
                        ci.CategoryColor = categoryColor;
                    }
                    await _context.SaveChangesAsync();
                }

                MenuItem item = finalType switch {
                    "Beverage" => new Beverage { Id = id, Name = vm.Name, Price = vm.Price,
                        Description = vm.Description, Category = finalCategory, CategoryColor = categoryColor,
                        ImageEmoji = vm.ImageEmoji, ImageUrl = imageUrl, IsAvailable = vm.IsAvailable,
                        IsHot = vm.IsHot, Size = vm.Size, OutletId = existingItem.OutletId },
                    "Food" => new Food { Id = id, Name = vm.Name, Price = vm.Price,
                        Description = vm.Description, Category = finalCategory, CategoryColor = categoryColor,
                        ImageEmoji = vm.ImageEmoji, ImageUrl = imageUrl, IsAvailable = vm.IsAvailable,
                        PreparationTimeMinutes = vm.PreparationTimeMinutes, IsVegetarian = vm.IsVegetarian, OutletId = existingItem.OutletId },
                    "Dessert" => new Dessert { Id = id, Name = vm.Name, Price = vm.Price,
                        Description = vm.Description, Category = finalCategory, CategoryColor = categoryColor,
                        ImageEmoji = vm.ImageEmoji, ImageUrl = imageUrl, IsAvailable = vm.IsAvailable,
                        Allergens = vm.Allergens, IsSeasonalItem = vm.IsSeasonalItem, OutletId = existingItem.OutletId },
                    "Snack" => new Snack { Id = id, Name = vm.Name, Price = vm.Price,
                        Description = vm.Description, Category = finalCategory, CategoryColor = categoryColor,
                        ImageEmoji = vm.ImageEmoji, ImageUrl = imageUrl, IsAvailable = vm.IsAvailable, OutletId = existingItem.OutletId },
                    "Special" => new Special { Id = id, Name = vm.Name, Price = vm.Price,
                        Description = vm.Description, Category = finalCategory, CategoryColor = categoryColor,
                        ImageEmoji = vm.ImageEmoji, ImageUrl = imageUrl, IsAvailable = vm.IsAvailable, OutletId = existingItem.OutletId },
                    _ => new Snack { Id = id, Name = vm.Name, Price = vm.Price,
                        Description = vm.Description, Category = finalCategory, CategoryColor = categoryColor,
                        ImageEmoji = vm.ImageEmoji, ImageUrl = imageUrl, IsAvailable = vm.IsAvailable, OutletId = existingItem.OutletId }
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

        public async Task<IActionResult> AllOrders(string? status, int? userId) {
            try {
                var orders = await _orderService.GetAllOrdersAsync();
                
                if (userId.HasValue) {
                    orders = orders.Where(o => o.UserId == userId.Value).ToList();
                    var user = await _context.Users.FindAsync(userId.Value);
                    ViewBag.CustomerName = user?.Name;
                }

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

        public async Task<IActionResult> UserList() {
            var users = await _context.Users
                .Include(u => u.Orders)
                .Where(u => u.Role == UserRole.Customer)
                .OrderByDescending(u => u.CreatedAt)
                .ToListAsync();
            return View(users);
        }

        public async Task<IActionResult> TableList() =>
            View(await _context.CafeTables.ToListAsync());

        // 🏢 Phase 5: Multi-Outlet Management
        public async Task<IActionResult> OutletList() =>
            View(await _context.Outlets.Include(o => o.Tables).Include(o => o.MenuItems).ToListAsync());

        [HttpGet] public IActionResult AddOutlet() => View();

        [HttpPost]
        public async Task<IActionResult> AddOutlet(CafeOutlet outlet) {
            if (!ModelState.IsValid) return View(outlet);
            _context.Outlets.Add(outlet);
            await _context.SaveChangesAsync();
            TempData["Success"] = "New outlet added successfully!";
            return RedirectToAction("OutletList");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteOutlet(int id) {
            var outlet = await _context.Outlets.FindAsync(id);
            if (outlet == null) return NotFound();
            _context.Outlets.Remove(outlet);
            await _context.SaveChangesAsync();
            return RedirectToAction("OutletList");
        }

        private string GetCategoryColor(string category) {
            return category.ToLower().Trim() switch {
                "beverages" or "beverage" or "drink" or "drinks" => "#673ab7", // Deep Purple
                "desserts" or "dessert" or "sweet" or "sweets" => "#e91e63",   // Pink
                "food" or "foods" or "meal" or "meals" => "#4caf50",          // Green
                "snacks" or "snack" => "#ff9800",                             // Orange
                "specials" or "special" => "#00bcd4",                         // Cyan
                _ => "#607d8b"                                                // Blue Grey
            };
        }
    }
}

