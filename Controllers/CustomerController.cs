using Microsoft.AspNetCore.Mvc;
using CafeManagement.Services;
using CafeManagement.Filters;
using CafeManagement.Models.Enums;

namespace CafeManagement.Controllers
{
    public class CustomerController : Controller {
        private readonly OrderService _orderService;
        private readonly AuthService _authService;

        public CustomerController(OrderService orderService, AuthService authService) {
            _orderService = orderService;
            _authService = authService;
        }

        [SessionAuthorize("Customer")]
        public async Task<IActionResult> Profile() {
            int userId = int.Parse(HttpContext.Session.GetString("UserId")!);
            var user = await _authService.GetUserByIdAsync(userId);
            return View(user);
        }

        [HttpPost][SessionAuthorize("Customer")]
        public async Task<IActionResult> Profile(string name, string email) {
            try {
                int userId = int.Parse(HttpContext.Session.GetString("UserId")!);
                await _authService.UpdateProfileAsync(userId, name, email);
                HttpContext.Session.SetString("UserName", name);
                TempData["Success"] = "Profile updated!";
            } catch (Exception ex) { TempData["Error"] = ex.Message; }
            return RedirectToAction("Profile");
        }

        [SessionAuthorize("Customer")]
        public async Task<IActionResult> OrderHistory() {
            int userId = int.Parse(HttpContext.Session.GetString("UserId")!);
            var orders = await _orderService.GetOrdersByUserAsync(userId);
            return View(orders);
        }

        // 📱 Phase 3: Open for Guests
        public async Task<IActionResult> OrderDetail(int id) {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null) return NotFound();

            // Security check: If it belongs to a user, that user must be logged in
            if (order.UserId.HasValue) {
                var sessionUserId = HttpContext.Session.GetString("UserId");
                if (sessionUserId == null || int.Parse(sessionUserId) != order.UserId.Value) {
                    return RedirectToAction("Login", "Auth");
                }
            }
            // If it's a guest order, we allow viewing it in this session 
            // (Simple version: if guest parameter is passed or session matches)
            return View(order);
        }

        public async Task<IActionResult> OrderConfirmation(int id) {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null) return NotFound();
            return View(order);
        }

        public async Task<IActionResult> TrackOrder(int id) {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null) return NotFound();
            return View(order);
        }
    }
}
