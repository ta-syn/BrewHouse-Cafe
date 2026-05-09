using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace CafeManagement.Models
{
    // 🏢 Phase 5.2: Multi-Outlet Support
    public class CafeOutlet
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        
        public string Location { get; set; } = string.Empty;
        public string ContactNumber { get; set; } = string.Empty;
        public string? ImageUrl { get; set; } // 🖼️ Added for visual appeal
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Relationships
        public virtual ICollection<MenuItem> MenuItems { get; set; } = new List<MenuItem>();
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
        public virtual ICollection<InventoryItem> InventoryItems { get; set; } = new List<InventoryItem>();
        public virtual ICollection<CafeTable> Tables { get; set; } = new List<CafeTable>();
    }
}
