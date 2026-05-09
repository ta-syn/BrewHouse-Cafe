using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CafeManagement.Models
{
    public abstract class MenuItem
    {
        public int Id { get; set; }

        // 🏢 Phase 5: Multi-Outlet Support
        public int? OutletId { get; set; }
        public CafeOutlet? Outlet { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Range(0, 100000)]
        public decimal Price { get; set; }

        public string Description { get; set; } = string.Empty;

        public string? ImageUrl { get; set; }
        public string? ImageEmoji { get; set; }

        public bool IsAvailable { get; set; } = true;

        [Required]
        public string Category { get; set; } = "General";
        
        public string CategoryColor { get; set; } = "#2C1810";

        public virtual ICollection<RecipeItem> Recipes { get; set; } = new List<RecipeItem>();

        // Required for Polymorphic display logic in some views
        public virtual string GetDescription() => Description;

        public virtual decimal ApplyDiscount(decimal percentage) {
            return Price - (Price * (percentage / 100));
        }
    }
}
