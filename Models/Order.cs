using System.ComponentModel.DataAnnotations;
using CafeManagement.Models.Enums;

namespace CafeManagement.Models
{
    public class Order {
        public int Id { get; set; }
        public int? UserId { get; set; }  // FIX: nullable for walk-in customers
        [Required]
        public string CustomerName { get; set; } = string.Empty;
        [Range(0, 999999)] public decimal TotalAmount { get; set; }
        public decimal DiscountApplied { get; set; } = 0;
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public string Notes { get; set; } = string.Empty;
        public bool IsWalkIn { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public User? User { get; set; }
        public int? TableId { get; set; }
        public CafeTable? Table { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
