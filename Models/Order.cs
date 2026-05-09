using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CafeManagement.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace CafeManagement.Models
{
    public class Order {
        public int Id { get; set; }
        
        // 🏢 Phase 5: Multi-Outlet Support
        public int? OutletId { get; set; }
        public CafeOutlet? Outlet { get; set; }

        public int? UserId { get; set; }
        [ForeignKey("UserId")]
        public User? User { get; set; }

        [Required]
        public string CustomerName { get; set; } = string.Empty;
        [Range(0, 999999)] public decimal TotalAmount { get; set; }
        public decimal DiscountApplied { get; set; } = 0;
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        
        public string PaymentStatus { get; set; } = "Pending";
        public string? PaymentMethod { get; set; }
        public string? TransactionId { get; set; }

        public DateTime? CompletedAt { get; set; }
        
        public int? CompletedByUserId { get; set; }
        [ForeignKey("CompletedByUserId")]
        public User? CompletedBy { get; set; }

        public string Notes { get; set; } = string.Empty;
        public bool IsWalkIn { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public int? TableId { get; set; }
        public CafeTable? Table { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
