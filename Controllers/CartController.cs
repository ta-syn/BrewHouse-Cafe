using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CafeManagement.Data;
using CafeManagement.Models.Session;
using CafeManagement.Services;
using CafeManagement.Filters;
using System.Text.Json;

namespace CafeManagement.Controllers
{
    public class CartController : Controller {
        private readonly MenuService _menuService;
        private readonly OrderService _orderService;
        private readonly CafeDbContext _context;

        public CartController(MenuService menuService, OrderService orderService, CafeDbContext context) {
            _menuService = menuService;
            _orderService = orderService;
            _context = context;
        }

        private List<CartItem> GetCart() {
            var json = HttpContext.Session.GetString("Cart");
            return string.IsNullOrEmpty(json)
                ? new List<CartItem>()
                : JsonSerializer.Deserialize<List<CartItem>>(json) ?? new();
        }

        private void SaveCart(List<CartItem> cart) {
            HttpContext.Session.SetString("Cart", JsonSerializer.Serialize(cart));
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int itemId, int quantity = 1) {
            try {
                var item = await _menuService.GetByIdAsync(itemId);
                if (item == null) return Json(new { success = false, message = "Item not found." });
                var cart = GetCart();
                var existing = cart.FirstOrDefault(c => c.MenuItemId == itemId);
                if (existing != null)
                    existing.Quantity += quantity;
                else
                    cart.Add(new CartItem {
                        MenuItemId = item.Id, ItemName = item.Name,
                        UnitPrice = item.Price, Quantity = quantity, 
                        ImageEmoji = item.ImageEmoji, ImageUrl = item.ImageUrl
                    });
                SaveCart(cart);
                return Json(new { success = true, cartCount = cart.Sum(c => c.Quantity) });
            } catch (Exception ex) {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost] public IActionResult RemoveFromCart(int itemId) {
            var cart = GetCart();
            cart.RemoveAll(c => c.MenuItemId == itemId);
            SaveCart(cart);
            return RedirectToAction("ViewCart");
        }

        [HttpPost] public IActionResult UpdateQuantity(int itemId, int quantity) {
            var cart = GetCart();
            var item = cart.FirstOrDefault(c => c.MenuItemId == itemId);
            if (item != null) {
                if (quantity <= 0) cart.Remove(item);
                else item.Quantity = quantity;
            }
            SaveCart(cart);
            return RedirectToAction("ViewCart");
        }

        public IActionResult ViewCart() {
            var cart = GetCart();
            var discountCode = HttpContext.Session.GetString("DiscountCode") ?? "";
            var discountPct = decimal.TryParse(HttpContext.Session.GetString("DiscountPercent"), out var d) ? d : 0;
            
            ViewBag.DiscountCode = discountCode;
            ViewBag.DiscountPercent = discountPct;
            ViewBag.Subtotal = cart.Sum(c => c.Subtotal);
            ViewBag.Total = ViewBag.Subtotal - (ViewBag.Subtotal * discountPct / 100);
            
            return View(cart);
        }

        [HttpPost]
        public async Task<IActionResult> ApplyDiscount(string code) {
            try {
                var discount = await _context.Discounts
                    .FirstOrDefaultAsync(d => d.Code.ToUpper() == code.ToUpper()
                        && d.IsActive && d.ExpiryDate >= DateTime.UtcNow);
                if (discount == null) {
                    TempData["Error"] = "Invalid or expired discount code.";
                } else {
                    HttpContext.Session.SetString("DiscountCode", code);
                    HttpContext.Session.SetString("DiscountPercent", discount.Percentage.ToString());
                    TempData["Success"] = $"{discount.Percentage}% discount applied!";
                }
            } catch (Exception ex) { TempData["Error"] = ex.Message; }
            return RedirectToAction("ViewCart");
        }

        public IActionResult GetCartCount() {
            var cart = GetCart();
            return Json(new { count = cart.Sum(c => c.Quantity) });
        }

        [SessionAuthorize("Customer")]
        public async Task<IActionResult> Checkout() {
            var cart = GetCart();
            if (!cart.Any()) return RedirectToAction("ViewCart");
            var discountPct = decimal.TryParse(HttpContext.Session.GetString("DiscountPercent"), out var d) ? d : 0;
            
            ViewBag.DiscountPercent = discountPct;
            ViewBag.Subtotal = cart.Sum(c => c.Subtotal);
            ViewBag.Total = ViewBag.Subtotal - (ViewBag.Subtotal * discountPct / 100);
            ViewBag.Tables = await _context.CafeTables.OrderBy(t => t.TableNumber).ToListAsync();
            
            return View(cart);
        }

        [HttpPost][SessionAuthorize("Customer")]
        public async Task<IActionResult> Checkout(string? notes, int? tableId) {
            try {
                var cart = GetCart();
                if (!cart.Any()) return RedirectToAction("ViewCart");
                
                int userId = int.Parse(HttpContext.Session.GetString("UserId")!);
                string userName = HttpContext.Session.GetString("UserName") ?? "Customer";
                string? discountCode = HttpContext.Session.GetString("DiscountCode");
                
                var order = await _orderService.CreateOrderAsync(userId, userName, cart, discountCode, tableId);
                
                // Clear cart and discount after order
                HttpContext.Session.Remove("Cart");
                HttpContext.Session.Remove("DiscountCode");
                HttpContext.Session.Remove("DiscountPercent");
                
                return RedirectToAction("OrderConfirmation", "Customer", new { id = order.Id });
            } catch (Exception ex) {
                TempData["Error"] = ex.Message;
                return RedirectToAction("ViewCart");
            }
        }
    }
}
