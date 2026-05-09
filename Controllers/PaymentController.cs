using Microsoft.AspNetCore.Mvc;
using CafeManagement.Data;
using CafeManagement.Models;
using CafeManagement.Models.Enums;
using CafeManagement.Services;
using Microsoft.EntityFrameworkCore;

namespace CafeManagement.Controllers
{
    public class PaymentController : Controller
    {
        private readonly CafeDbContext _context;
        private readonly OrderService _orderService;

        public PaymentController(CafeDbContext context, OrderService orderService)
        {
            _context = context;
            _orderService = orderService;
        }

        [HttpGet]
        public async Task<IActionResult> Gateway(int orderId, string method, string? itemIds)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null) return NotFound();

            // 💸 Phase 3: Split Bill Logic
            if (!string.IsNullOrEmpty(itemIds))
            {
                var idList = itemIds.Split(',').Select(int.Parse).ToList();
                var selectedItems = order.OrderItems.Where(oi => idList.Contains(oi.Id)).ToList();
                ViewBag.SelectedItems = selectedItems;
                ViewBag.SplitTotal = selectedItems.Sum(s => s.Subtotal);
                ViewBag.ItemIds = itemIds;
            }

            ViewBag.Method = method;
            return View(order);
        }

        [HttpPost]
        public async Task<IActionResult> ProcessMockPayment(int orderId, string method, string accountNumber, string? itemIds)
        {
            await Task.Delay(2000);

            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order != null)
            {
                string trxId = "TRX-" + Guid.NewGuid().ToString().Substring(0, 8).ToUpper();

                if (!string.IsNullOrEmpty(itemIds))
                {
                    // 🍕 Partial Payment (Split Bill)
                    var idList = itemIds.Split(',').Select(int.Parse).ToList();
                    foreach (var item in order.OrderItems.Where(oi => idList.Contains(oi.Id)))
                    {
                        item.IsPaid = true;
                    }
                    
                    // If all items are paid, mark order as paid
                    if (order.OrderItems.All(oi => oi.IsPaid)) {
                        order.PaymentStatus = "Paid";
                        order.PaymentMethod = method;
                        order.TransactionId = trxId;
                    } else {
                        order.PaymentStatus = "Partially Paid";
                    }
                }
                else
                {
                    // 💰 Full Payment
                    order.PaymentStatus = "Paid";
                    order.PaymentMethod = method;
                    order.TransactionId = trxId;
                    foreach (var item in order.OrderItems) item.IsPaid = true;
                }

                await _context.SaveChangesAsync();
                return Json(new { success = true, trxId = trxId });
            }

            return Json(new { success = false, message = "Order not found." });
        }
    }
}
