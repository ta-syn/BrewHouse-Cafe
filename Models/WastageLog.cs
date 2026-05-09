using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CafeManagement.Models
{
    public class WastageLog
    {
        public int Id { get; set; }

        [Required]
        public int InventoryItemId { get; set; }

        [ForeignKey("InventoryItemId")]
        public virtual InventoryItem? InventoryItem { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Quantity { get; set; }

        [Required]
        [StringLength(255)]
        public string Reason { get; set; } = string.Empty; // e.g., Spilled, Expired, Spoiled

        public DateTime RecordedAt { get; set; } = DateTime.UtcNow.AddHours(6); // Bangladesh Time
    }
}
