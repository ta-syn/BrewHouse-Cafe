namespace CafeManagement.Models
{
    public class OrderItem {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int MenuItemId { get; set; }
        public string ItemName { get; set; } = string.Empty;  // snapshot at order time
        public decimal UnitPrice { get; set; }                 // FIX: snapshot price, not live price
        public int Quantity { get; set; }
        public Order? Order { get; set; }
        public MenuItem? MenuItem { get; set; }
        public decimal Subtotal => Quantity * UnitPrice;       // computed
    }
}
