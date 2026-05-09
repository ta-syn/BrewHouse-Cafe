using Microsoft.EntityFrameworkCore;
using CafeManagement.Data;
using CafeManagement.Models;
using Microsoft.AspNetCore.SignalR;
using CafeManagement.Hubs;

namespace CafeManagement.Services
{
    public class InventoryService
    {
        private readonly CafeDbContext _context;
        private readonly IHubContext<OrderHub> _hubContext;
        private readonly EmailService _emailService;

        public InventoryService(CafeDbContext context, IHubContext<OrderHub> hubContext, EmailService emailService)
        {
            _context = context;
            _hubContext = hubContext;
            _emailService = emailService;
        }

        public async Task DeductStockForOrderAsync(int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.MenuItem)
                .ThenInclude(m => m.Recipes)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null) return;

            foreach (var orderItem in order.OrderItems)
            {
                if (orderItem.MenuItem == null) continue;

                foreach (var recipe in orderItem.MenuItem.Recipes)
                {
                    var inventoryItem = await _context.InventoryItems.FindAsync(recipe.InventoryItemId);
                    if (inventoryItem != null)
                    {
                        decimal amountToDeduct = recipe.QuantityRequired * orderItem.Quantity;
                        inventoryItem.StockQuantity -= amountToDeduct;
                        inventoryItem.LastUpdated = DateTime.UtcNow;

                        if (inventoryItem.StockQuantity <= inventoryItem.MinStockLevel)
                        {
                            await NotifyLowStockAsync(inventoryItem);
                        }
                    }
                }
            }

            await _context.SaveChangesAsync();
        }

        public async Task RecordWastageAsync(int itemId, decimal quantity, string reason)
        {
            var item = await _context.InventoryItems.FindAsync(itemId);
            if (item == null) return;

            item.StockQuantity -= quantity;
            item.LastUpdated = DateTime.UtcNow;

            var log = new WastageLog
            {
                InventoryItemId = itemId,
                Quantity = quantity,
                Reason = reason,
                RecordedAt = DateTime.UtcNow.AddHours(6)
            };

            _context.WastageLogs.Add(log);
            await _context.SaveChangesAsync();

            if (item.StockQuantity <= item.MinStockLevel)
            {
                await NotifyLowStockAsync(item);
            }
        }

        private async Task NotifyLowStockAsync(InventoryItem item)
        {
            // 1. 🛰️ SIGNALR: Live Warning
            await _hubContext.Clients.Groups("Admin").SendAsync("ReceiveLowStockWarning", new
            {
                itemId = item.Id,
                itemName = item.Name,
                currentStock = item.StockQuantity,
                unit = item.Unit,
                minLevel = item.MinStockLevel
            });

            // 2. 📝 DATABASE: Save Notification
            var notification = new Notification
            {
                Title = "Low Stock Alert",
                Message = $"Ingredient '{item.Name}' is low. Current: {item.StockQuantity}{item.Unit}",
                Type = "Warning",
                TargetUrl = "/Admin/Inventory"
            };
            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            // 3. 📧 EMAIL: Send Alert to Owner
            await _emailService.SendLowStockEmailAsync(item.Name, item.StockQuantity, item.Unit);
        }

        public async Task<List<InventoryItem>> GetAllInventoryAsync() =>
            await _context.InventoryItems.OrderBy(i => i.Name).ToListAsync();

        public async Task UpdateStockAsync(int itemId, decimal newQuantity)
        {
            var item = await _context.InventoryItems.FindAsync(itemId);
            if (item != null)
            {
                item.StockQuantity = newQuantity;
                item.LastUpdated = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }
    }
}
