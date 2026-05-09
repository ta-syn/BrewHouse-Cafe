using Microsoft.EntityFrameworkCore;
using CafeManagement.Data;
using CafeManagement.Models;
using CafeManagement.Models.Enums;
using CafeManagement.Models.Session;
using CafeManagement.Exceptions;
using Microsoft.AspNetCore.SignalR;
using CafeManagement.Hubs;

namespace CafeManagement.Services
{
    public class OrderService {
        private readonly CafeDbContext _context;
        private readonly IHubContext<OrderHub> _hubContext;
        private readonly InventoryService _inventoryService;

        public OrderService(CafeDbContext context, IHubContext<OrderHub> hubContext, InventoryService inventoryService) { 
            _context = context; 
            _hubContext = hubContext;
            _inventoryService = inventoryService;
        }

        public async Task<Order> CreateOrderAsync(
                int? userId, string customerName,
                List<CartItem> cartItems, string? discountCode, int? tableId = null, string? notes = null) {
            try {
                if (cartItems == null || cartItems.Count == 0)
                    throw new OrderException("Cart is empty.");

                decimal discountPercent = 0;
                if (!string.IsNullOrEmpty(discountCode)) {
                    var discount = await _context.Discounts
                        .FirstOrDefaultAsync(d =>
                            d.Code.ToUpper() == discountCode.ToUpper() &&
                            d.IsActive &&
                            d.ExpiryDate >= DateTime.UtcNow);
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
                    Notes = notes ?? string.Empty,
                    IsWalkIn = userId == null,
                    CreatedAt = DateTime.UtcNow.AddHours(6),
                    OrderItems = cartItems.Select(c => new OrderItem {
                        MenuItemId = c.MenuItemId,
                        ItemName = c.ItemName,
                        UnitPrice = c.UnitPrice,
                        Quantity = c.Quantity
                    }).ToList()
                };

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                await _hubContext.Clients.Groups("Staff", "Admin").SendAsync("ReceiveOrderNotification", new {
                    orderId = order.Id,
                    customerName = order.CustomerName,
                    amount = order.TotalAmount,
                    table = order.TableId.HasValue ? order.TableId.ToString() : "Walk-in",
                    time = order.CreatedAt.ToString("hh:mm tt")
                });

                return order;

            } catch (CafeException) { throw; }
            catch (Exception ex) {
                Console.WriteLine($"[{DateTime.UtcNow.AddHours(6)}] CreateOrder Error: {ex.Message}");
                throw new OrderException("Failed to create order.");
            }
        }

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

        // 📊 Phase 4: Updated to include staff tracking
        public async Task UpdateStatusAsync(int orderId, OrderStatus status, int? staffUserId = null) {
            try {
                var order = await _context.Orders.FindAsync(orderId);
                if (order == null) throw new ItemNotFoundException("Order not found.");

                if (order.Status == OrderStatus.Served || order.Status == OrderStatus.Cancelled)
                    throw new OrderException($"This order is already {order.Status} and its status cannot be changed further.");

                order.Status = status;
                
                // Track completion for analytics
                if (status == OrderStatus.Served)
                {
                    order.CompletedAt = DateTime.UtcNow.AddHours(6);
                    order.CompletedByUserId = staffUserId;
                    await _inventoryService.DeductStockForOrderAsync(orderId);
                }

                await _context.SaveChangesAsync();

                string orderGroupName = $"Order_{order.Id}";
                await _hubContext.Clients.Groups(orderGroupName, "Staff", "Admin").SendAsync("UpdateOrderStatus", new {
                    orderId = order.Id,
                    status = status.ToString(),
                    customerName = order.CustomerName
                });

            } catch (CafeException) { throw; }
            catch (Exception ex) {
                Console.WriteLine($"[{DateTime.Now}] UpdateStatus Error: {ex.Message}");
                throw;
            }
        }

        public async Task CancelOrderAsync(int orderId, int requestingUserId, bool isAdmin) {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null) throw new ItemNotFoundException("Order not found.");
            if (!isAdmin && order.UserId != requestingUserId)
                throw new UnauthorizedCafeAccessException("You cannot cancel this order.");
            if (order.Status == OrderStatus.Served)
                throw new OrderException("Cannot cancel a served order.");
            order.Status = OrderStatus.Cancelled;
            await _context.SaveChangesAsync();
        }
    }
}
