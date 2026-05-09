namespace CafeManagement.Models
{
    public class OrderItem {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int MenuItemId { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public bool IsPaid { get; set; } = false; // 💸 Phase 3: Split Bill Support
        
        public Order? Order { get; set; }
        public MenuItem? MenuItem { get; set; }
        public decimal Subtotal => Quantity * UnitPrice;
    }
}
