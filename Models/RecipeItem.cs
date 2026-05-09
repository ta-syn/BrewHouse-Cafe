using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CafeManagement.Models
{
    public class RecipeItem
    {
        public int Id { get; set; }

        [Required]
        public int MenuItemId { get; set; }
        
        [ForeignKey("MenuItemId")]
        public virtual MenuItem? MenuItem { get; set; }

        [Required]
        public int InventoryItemId { get; set; }

        [ForeignKey("InventoryItemId")]
        public virtual InventoryItem? InventoryItem { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal QuantityRequired { get; set; }
    }
}
