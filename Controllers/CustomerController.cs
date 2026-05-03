using Microsoft.AspNetCore.Mvc;
using CafeManagement.Services;
using CafeManagement.Filters;
using CafeManagement.Models.Enums;

namespace CafeManagement.Controllers
{
    [SessionAuthorize("Customer")]
    public class CustomerController : Controller {
        private readonly OrderService _orderService;
        private readonly AuthService _authService;

        public CustomerController(OrderService orderService, AuthService authService) {
            _orderService = orderService;
            _authService = authService;
        }

        public async Task<IActionResult> Profile() {
            int userId = int.Parse(HttpContext.Session.GetString("UserId")!);
            var user = await _authService.GetUserByIdAsync(userId);
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> Profile(string name, string email) {
            try {
                int userId = int.Parse(HttpContext.Session.GetString("UserId")!);
                await _authService.UpdateProfileAsync(userId, name, email);
                HttpContext.Session.SetString("UserName", name);
                TempData["Success"] = "Profile updated!";
            } catch (Exception ex) { TempData["Error"] = ex.Message; }
            return RedirectToAction("Profile");
        }

        public async Task<IActionResult> OrderHistory() {
            int userId = int.Parse(HttpContext.Session.GetString("UserId")!);
            var orders = await _orderService.GetOrdersByUserAsync(userId);
            return View(orders);
        }

        public async Task<IActionResult> OrderDetail(int id) {
            int userId = int.Parse(HttpContext.Session.GetString("UserId")!);
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null || order.UserId != userId) {
                TempData["Error"] = "Order not found.";
                return RedirectToAction("OrderHistory");
            }
            return View(order);
        }

        public async Task<IActionResult> OrderConfirmation(int id) {
            int userId = int.Parse(HttpContext.Session.GetString("UserId")!);
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null || order.UserId != userId) return RedirectToAction("OrderHistory");
            return View(order);
        }

        public async Task<IActionResult> TrackOrder(int id) {
            int userId = int.Parse(HttpContext.Session.GetString("UserId")!);
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null || order.UserId != userId) return RedirectToAction("OrderHistory");
            return View(order);
        }
    }
}
