using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace CafeManagement.Models
{
    public class InventoryItem
    {
        public int Id { get; set; }

        // 🏢 Phase 5: Multi-Outlet Support
        public int? OutletId { get; set; }
        public CafeOutlet? Outlet { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public decimal StockQuantity { get; set; }

        [Required]
        [StringLength(20)]
        public string Unit { get; set; } = "g";

        public decimal MinStockLevel { get; set; }

        public string? ImageUrl { get; set; } // 🖼️ Added for visual tracking
        public DateTime? ExpiryDate { get; set; }
        public decimal UnitCost { get; set; }

        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

        public virtual ICollection<RecipeItem> RecipeItems { get; set; } = new List<RecipeItem>();
    }
}
