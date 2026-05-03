// FIX: CartItem is session-only, NOT a database model
// Do NOT add this to DbContext
namespace CafeManagement.Models.Session {
    public class CartItem {
        public int MenuItemId { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public string ImageEmoji { get; set; } = "☕";
        public string? ImageUrl { get; set; }
        public decimal Subtotal => Quantity * UnitPrice;
    }
}
