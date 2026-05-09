using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CafeManagement.Data;
using CafeManagement.Models;
using CafeManagement.Models.Enums;
using CafeManagement.Models.Session;
using CafeManagement.Services;
using CafeManagement.Filters;
using CafeManagement.Exceptions;
using Microsoft.AspNetCore.SignalR;
using CafeManagement.Hubs;
using System.Text.Json;

namespace CafeManagement.Controllers
{
    [SessionAuthorize("Staff", "Admin")]
    public class StaffController : Controller {
        private readonly OrderService _orderService;
        private readonly MenuService _menuService;
        private readonly CafeDbContext _context;
        private readonly IHubContext<OrderHub> _hubContext;

        public StaffController(OrderService orderService, MenuService menuService, CafeDbContext context, IHubContext<OrderHub> hubContext) {
            _orderService = orderService;
            _menuService = menuService;
            _context = context;
            _hubContext = hubContext;
        }

        public async Task<IActionResult> Dashboard() {
            var activeOrders = await _orderService.GetActiveOrdersAsync();
            var allOrders = await _orderService.GetAllOrdersAsync();
            var todayLocal = DateTime.UtcNow.AddHours(6).Date;
            var tomorrowLocal = todayLocal.AddDays(1);
            var todayOrders = allOrders.Where(o => o.CreatedAt >= todayLocal && o.CreatedAt < tomorrowLocal).ToList();
            
            ViewBag.PendingCount = activeOrders.Count(o => o.Status == OrderStatus.Pending);
            ViewBag.TodayCount = todayOrders.Count;
            ViewBag.TotalCount = allOrders.Count;
            ViewBag.TodayRevenue = todayOrders.Where(o => o.Status != OrderStatus.Cancelled).Sum(o => o.TotalAmount);
            
            return View(activeOrders);
        }

        public async Task<IActionResult> ActiveOrders() =>
            View(await _orderService.GetActiveOrdersAsync());

        public async Task<IActionResult> KDS() =>
            View(await _orderService.GetActiveOrdersAsync());

        public async Task<IActionResult> OrderDetail(int id) {
            try {
                var order = await _orderService.GetOrderByIdAsync(id);
                if (order == null) throw new ItemNotFoundException("Order not found.");
                return View(order);
            } catch (Exception ex) {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Dashboard");
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int orderId, string status) {
            try {
                if (!Enum.TryParse<OrderStatus>(status, out var orderStatus))
                    throw new OrderException("Invalid status.");
                
                await _orderService.UpdateStatusAsync(orderId, orderStatus);

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest") {
                    return Json(new { success = true, newStatus = status });
                }
                
                TempData["Success"] = "Order status updated!";
                return RedirectToAction("OrderDetail", new { id = orderId });
            } catch (Exception ex) {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest") {
                    return Json(new { success = false, message = ex.Message });
                }
                TempData["Error"] = ex.Message;
                return RedirectToAction("Dashboard");
            }
        }

        public async Task<IActionResult> TableOverview() =>
            View(await _context.CafeTables.ToListAsync());

        [HttpPost]
        public async Task<IActionResult> UpdateTableStatus(int tableId, string status) {
            try {
                if (!Enum.TryParse<TableStatus>(status, out var tableStatus))
                    throw new CafeException("Invalid status.");
                var table = await _context.CafeTables.FindAsync(tableId);
                if (table == null) throw new ItemNotFoundException("Table not found.");
                table.Status = tableStatus;
                await _context.SaveChangesAsync();

                // 🛰️ SIGNALR: Sync Table Status
                await _hubContext.Clients.All.SendAsync("ReceiveTableUpdate", new { 
                    tableId = table.Id, 
                    status = tableStatus.ToString(),
                    tableNumber = table.TableNumber 
                });

                TempData["Success"] = "Table updated.";
            } catch (Exception ex) { TempData["Error"] = ex.Message; }
            return RedirectToAction("TableOverview");
        }

        [HttpGet]
        public async Task<IActionResult> WalkInOrder() {
            ViewBag.Tables = await _context.CafeTables.OrderBy(t => t.TableNumber).ToListAsync();
            return View(await _menuService.GetAvailableItemsAsync());
        }

        [HttpPost]
        public async Task<IActionResult> WalkInOrder(string itemsJson, int? tableId, string? notes, string customerName = "Walk-in Customer") {
            try {
                var cartItems = JsonSerializer.Deserialize<List<CartItem>>(itemsJson);
                if (cartItems == null || !cartItems.Any())
                    throw new OrderException("No items selected.");
                var order = await _orderService.CreateOrderAsync(null, customerName, cartItems, null, tableId, notes);
                TempData["Success"] = $"Walk-in Order #{order.Id} created!";
                return RedirectToAction("ActiveOrders");
            } catch (Exception ex) {
                TempData["Error"] = ex.Message;
                return RedirectToAction("WalkInOrder");
            }
        }
    }
}
