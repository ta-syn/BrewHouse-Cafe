using Microsoft.EntityFrameworkCore;
using CafeManagement.Data;
using CafeManagement.Models;
using CafeManagement.Models.Enums;
using CafeManagement.Models.Session;
using CafeManagement.Exceptions;

namespace CafeManagement.Services
{
    public class OrderService {
        private readonly CafeDbContext _context;
        public OrderService(CafeDbContext context) { _context = context; }

        public async Task<Order> CreateOrderAsync(
                int? userId, string customerName,
                List<CartItem> cartItems, string? discountCode, int? tableId = null) {
            // ═══ OOP CONCEPT: EXCEPTION HANDLING ═══
            try {
                if (cartItems == null || cartItems.Count == 0)
                    throw new OrderException("Cart is empty.");

                // Validate and apply discount
                decimal discountPercent = 0;
                if (!string.IsNullOrEmpty(discountCode)) {
                    var discount = await _context.Discounts
                        .FirstOrDefaultAsync(d =>
                            d.Code.ToUpper() == discountCode.ToUpper() &&
                            d.IsActive &&
                            d.ExpiryDate >= DateTime.UtcNow);  // FIX: expiry check
                    if (discount != null)
                        discountPercent = discount.Percentage;
                }

                decimal subtotal = cartItems.Sum(c => c.Subtotal);
                decimal totalAmount = subtotal - (subtotal * discountPercent / 100);

                var order = new Order {
                    UserId = userId,
                    TableId = tableId,
                    CustomerName = customerName,
                    TotalAmount = totalAmount,
                    DiscountApplied = discountPercent,
                    Status = OrderStatus.Pending,
                    IsWalkIn = userId == null,
                    CreatedAt = DateTime.UtcNow.AddHours(6), // SYNC: Bangladesh Time (UTC+6)
                    OrderItems = cartItems.Select(c => new OrderItem {
                        MenuItemId = c.MenuItemId,
                        ItemName = c.ItemName,
                        UnitPrice = c.UnitPrice,
                        Quantity = c.Quantity
                    }).ToList()
                };

                // Update table status to Occupied if table selected
                if (tableId.HasValue) {
                    var table = await _context.CafeTables.FindAsync(tableId.Value);
                    if (table != null) table.Status = TableStatus.Occupied;
                }

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();
                return order;
            } catch (CafeException) { throw; }
            catch (Exception ex) {
                Console.WriteLine($"[{DateTime.UtcNow.AddHours(6)}] CreateOrder Error: {ex.Message}");
                throw new OrderException("Failed to create order.");
            }
        }

        // FIX: Use Include() for eager loading
        public async Task<Order?> GetOrderByIdAsync(int id) =>
            await _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.MenuItem)
                .Include(o => o.User)
                .Include(o => o.Table)
                .FirstOrDefaultAsync(o => o.Id == id);

        public async Task<List<Order>> GetOrdersByUserAsync(int userId) =>
            await _context.Orders
                .Include(o => o.OrderItems)
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

        public async Task<List<Order>> GetAllOrdersAsync() =>
            await _context.Orders
                .Include(o => o.OrderItems)
                .Include(o => o.User)
                .Include(o => o.Table)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

        public async Task<List<Order>> GetActiveOrdersAsync() =>
            await _context.Orders
                .Include(o => o.OrderItems)
                .Include(o => o.Table)
                .Where(o => o.Status == OrderStatus.Pending ||
                            o.Status == OrderStatus.Preparing ||
                            o.Status == OrderStatus.Ready)
                .OrderBy(o => o.CreatedAt)
                .ToListAsync();

        public async Task UpdateStatusAsync(int orderId, OrderStatus status) {
            // ═══ OOP CONCEPT: EXCEPTION HANDLING ═══
            try {
                var order = await _context.Orders.FindAsync(orderId);
                if (order == null) throw new ItemNotFoundException("Order not found.");

                // ═══ LOCK LOGIC: Served/Cancelled are terminal statuses ═══
                if (order.Status == OrderStatus.Served || order.Status == OrderStatus.Cancelled)
                    throw new OrderException($"This order is already {order.Status} and its status cannot be changed further.");

                order.Status = status;
                await _context.SaveChangesAsync();
            } catch (CafeException) { throw; }
            catch (Exception ex) {
                Console.WriteLine($"[{DateTime.Now}] UpdateStatus Error: {ex.Message}");
                throw;
            }
        }

        public async Task CancelOrderAsync(int orderId, int requestingUserId, bool isAdmin) {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null) throw new ItemNotFoundException("Order not found.");
            // FIX: ownership check
            if (!isAdmin && order.UserId != requestingUserId)
                throw new UnauthorizedCafeAccessException("You cannot cancel this order.");
            if (order.Status == OrderStatus.Served)
                throw new OrderException("Cannot cancel a served order.");
            order.Status = OrderStatus.Cancelled;
            await _context.SaveChangesAsync();
        }
    }
}
